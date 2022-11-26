using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppUserAccountManager : MonoBehaviour
    {
        public HoloKitAppSIWAManager SIWAManager;

        [SerializeField] private bool _clearSessionToken = false;

        public bool Authenticated => _authenticated;

        private bool _ugsInitialized = false;

        /// <summary>
        /// Whether the local user is currently logged in?
        /// </summary>
        private bool _authenticated = false;

        /// <summary>
        /// Whether the current logged in user's name and email are synced on the cloud?
        /// </summary>
        private bool _userInfoSynced = false;

        /// <summary>
        /// After a connection failure, we wait for a certian period of time and try again.
        /// The unit here is second.
        /// </summary>
        private const int RetryConnectionInterval = 30;

        /// <summary>
        /// This event is called when we start to initialize UGS.
        /// </summary>
        public static event Action OnInitializingUGS;

        /// <summary>
        /// This event is called when UGS initialization fails.
        /// </summary>
        public static event Action OnInitializingUGSFailed;

        /// <summary>
        /// This event is called when we attempt to sign in with a cached user.
        /// </summary>
        public static event Action OnSigningInWithCachedUser;

        public static event Action OnAuthenticatingAppleId;

        public static event Action OnAuthenticatingAppleIdSucceeded;

        public static event Action OnAuthenticatingAppleIdFailed;

        private void Start()
        {
            HoloKitAppSIWAManager.OnSignedInWithApple += OnSignedInWithApple;
        }

        private void OnDestroy()
        {
            HoloKitAppSIWAManager.OnSignedInWithApple -= OnSignedInWithApple;
        }

        private void SetupAuthenticationCallbacks()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                _authenticated = true;
                SyncUserInfo();
                OnAuthenticatingAppleIdSucceeded?.Invoke();
            };

            AuthenticationService.Instance.SignInFailed += (error) =>
            {
                _authenticated = false;
                OnAuthenticatingAppleIdFailed?.Invoke();
                Debug.LogError(error);
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                _authenticated = false;
                Debug.Log("Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                _authenticated = false;
                Debug.Log("Player session could not be refreshed and expired.");
            };
        }

        public async void StartProcess()
        {
            _ugsInitialized = await InitializeUGS();
            if (_ugsInitialized)
            {
                if (_clearSessionToken)
                    AuthenticationService.Instance.ClearSessionToken();
                SetupAuthenticationCallbacks();
                StartAuthentication();
            }
        }

        private async Task<bool> InitializeUGS()
        {
            try
            {
                OnInitializingUGS?.Invoke();
                await UnityServices.InitializeAsync();
                Debug.Log("UGS initialized");
                return true;
            }
            catch (Exception e)
            {
                OnInitializingUGSFailed?.Invoke();
                Debug.Log("Failed to initialize UGS");
                Debug.LogException(e);
                return false;
            }
        }

        private async void StartAuthentication()
        {
            // Check if a cached user already exists by checking if the session token exists
            if (!AuthenticationService.Instance.SessionTokenExists)
            {
                // Sign in with Apple
                SIWAManager.SignIn();
                return;
            }

            // Sign in Anonymously
            // This call will sign in the cached user.
            try
            {
                OnSigningInWithCachedUser?.Invoke();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException e)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
            catch (RequestFailedException e)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
        }

        private async void OnSignedInWithApple(string idToken)
        {
            try
            {
                OnAuthenticatingAppleId?.Invoke();
                await AuthenticationService.Instance.SignInWithAppleAsync(idToken);
            }
            catch (AuthenticationException e)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
            catch (RequestFailedException e)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(e);
            }
        }

        private async void SyncUserInfo()
        {
            // First we have to make sure the user info is stored in the local device
            // If we have the user email stored, we can be sure that we have all
            // the info about this user (this implies this was not a quick login).
            const string appleUserEmailKey = HoloKitAppSIWAManager.AppleUserEmailKey;
            if (PlayerPrefs.HasKey(appleUserEmailKey))
            {
                Debug.Log("[UserAccountManager] Found user info on the current device");
                // Then we want to check whether the user info is already stored in the cloud
                var keySet = new HashSet<string> { appleUserEmailKey };
                try
                {
                    Debug.Log("[UserAccountManager] Fetching user info");
                    Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(keySet);
                    if (savedData.ContainsKey(appleUserEmailKey))
                    {
                        // The user info has already been synced in the cloud
                        _userInfoSynced = true;
                        Debug.Log("[UserAccountManager] User info fetched");
                    }
                    else
                    {
                        _userInfoSynced = false;
                        Debug.Log("[UserAccountManager] User info not stored in the cloud");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.Log("[UserAccountManager] Failed to get user data, let's try again...");
                    await Task.Delay(RetryConnectionInterval * 1000);
                    SyncUserInfo();
                    return;
                }

                if (!_userInfoSynced)
                {
                    const string appleUserIdKey = HoloKitAppSIWAManager.AppleUserIdKey;
                    const string appleUserFullNameKey = HoloKitAppSIWAManager.AppleUserFullNameKey;
                    var data = new Dictionary<string, object>
                    {
                        { appleUserIdKey, PlayerPrefs.GetString(appleUserIdKey) },
                        { appleUserFullNameKey, PlayerPrefs.HasKey(appleUserFullNameKey) ? PlayerPrefs.GetString(appleUserFullNameKey) : "Anonymous" },
                        { appleUserEmailKey, PlayerPrefs.GetString(appleUserEmailKey) }
                    };
                    try
                    {
                        Debug.Log("[UserAccountManager] Uploading user info");
                        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
                        _userInfoSynced = true;
                        Debug.Log("[UserAccountManager] User info uploaded");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Debug.Log("[UserAccountManager] Failed to upload user data, let's try again...");
                        await Task.Delay(RetryConnectionInterval * 1000);
                        SyncUserInfo();
                        return;
                    }
                }
            }
            else {
                Debug.Log("[UserAccountManager] User info not found on the current device");
            }
        }
    }
}
