// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

#if UNITY_SERVICES_CORE_ENABLED

using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib
{
    /// <summary>
    /// This part of the partial class is responsible for Analytics.
    /// </summary>
    public partial class HoloKitAppUserAccountManager
    {
        private const string RealityBundleIdKey = "realityBundleId";

        private const string SessionDurationKey = "sessionDuration";

        private const string PlayerCountKey = "playerCount";

        private const string IsHostKey = "isHost";

        private const string PlayerTypeKey = "playerType";

        private const string UserEmailKey = "userEmail";

        private const string UserNameKey = "userName";

        // We cannot initialize Analytics when there is no network connection
        private async void Analytics_Init()
        {
            // try
            // {
            //     List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
            // }
            // catch (ConsentCheckException e)
            // {
                
            // }
            
        	AnalyticsService.Instance.StartDataCollection();

            HoloKitAppAnalyticsEventManager.OnPlayerRegistered += OnPlayerRegistered;
            HoloKitAppAnalyticsEventManager.OnDreamOver += OnDreamOver;
            HoloKitAppAnalyticsEventManager.OnOverheated += OnOverheated;
            HoloKitCameraManager.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void Analytics_Deinit()
        {
            HoloKitAppAnalyticsEventManager.OnPlayerRegistered -= OnPlayerRegistered;
            HoloKitAppAnalyticsEventManager.OnDreamOver -= OnDreamOver;
            HoloKitAppAnalyticsEventManager.OnOverheated -= OnOverheated;
            HoloKitCameraManager.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        private void OnDreamOver(RealitySessionData realitySessionData)
        {
            CustomEvent e = new CustomEvent("dreamOver") 
            {
                { RealityBundleIdKey, realitySessionData.RealityBundleId },
                { SessionDurationKey, realitySessionData.SessionDuration },
                { PlayerCountKey, realitySessionData.PlayerCount }
            };

            try
            {
                AnalyticsService.Instance.RecordEvent(e);
            }
            catch (Exception)
            {

            }
        }

        private void OnOverheated(HoloKitAppOverheatData overheatData)
        {
            CustomEvent e = new CustomEvent("overheat") 
            {
                { RealityBundleIdKey, overheatData.RealitySessionData.RealityBundleId },
                { SessionDurationKey, overheatData.RealitySessionData.SessionDuration },
                { PlayerCountKey, overheatData.RealitySessionData.PlayerCount },
                { IsHostKey, overheatData.IsHost },
                { PlayerTypeKey, overheatData.PlayerType }
            };

            try
            {
                AnalyticsService.Instance.RecordEvent(e);
            }
            catch (Exception)
            {

            }
        }

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                CustomEvent e = new CustomEvent("enterStarMode") {};

                try
                {
                    AnalyticsService.Instance.RecordEvent(e);
                }
                catch (Exception)
                {

                }
            }
        }

        private void OnPlayerRegistered(string userEmail, string userName)
        {
            CustomEvent e = new CustomEvent("playerRegistered")
            {
                { UserEmailKey, userEmail },
                { UserNameKey, userName }
            };

            try
            {
                AnalyticsService.Instance.RecordEvent(e);
            }
            catch (Exception)
            {

            }
        }
    }
}

#endif