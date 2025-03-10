// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using RealityDesignLab.Library.Permissions;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_PermissionsPage_DoneButton : MonoBehaviour
    {
        [SerializeField] private Button _doneButton;

        private void Awake()
        {
            PermissionsAPI.OnRequestCameraPermissionCompleted += OnRequestCameraPermissionCompleted;
            PermissionsAPI.OnRequestMicrophonePermissionCompleted += OnRequestMicrophonePermissionCompleted;
            PermissionsAPI.OnRequestPhotoLibraryAddPermissionCompleted += OnRequestPhotoLibraryAddPermissionCompleted;
            PermissionsAPI.OnLocationPermissionStatusChanged += OnLocationPermissionStatusChanged;
        }

        private void Start()
        {
            if (HoloKitUtils.IsRuntime)
            {
                _doneButton.interactable = false;
            }
            else
            {
                _doneButton.interactable = true;
            }
        }

        private void OnDestroy()
        {
            PermissionsAPI.OnRequestCameraPermissionCompleted -= OnRequestCameraPermissionCompleted;
            PermissionsAPI.OnRequestMicrophonePermissionCompleted -= OnRequestMicrophonePermissionCompleted;
            PermissionsAPI.OnRequestPhotoLibraryAddPermissionCompleted -= OnRequestPhotoLibraryAddPermissionCompleted;
            PermissionsAPI.OnLocationPermissionStatusChanged -= OnLocationPermissionStatusChanged;
        }

        private void OnRequestCameraPermissionCompleted(bool _)
        {
            UpdateDoneButtonInteractivity();
        }

        private void OnRequestMicrophonePermissionCompleted(bool _)
        {
            UpdateDoneButtonInteractivity();
        }

        private void OnRequestPhotoLibraryAddPermissionCompleted(PhotoLibraryPermissionStatus _)
        {
            UpdateDoneButtonInteractivity();
        }

        private void OnLocationPermissionStatusChanged(LocationPermissionStatus _)
        {
            UpdateDoneButtonInteractivity();
        }

        private void UpdateDoneButtonInteractivity()
        {
            if (HoloKitAppPermissionsManager.MandatoryPermissionsGranted())
            {
                _doneButton.interactable = true;
            }
        }

        public void OnDoneButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("SignInPage");
        }
    }
}
