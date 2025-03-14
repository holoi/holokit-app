// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using RealityDesignLab.Library.Permissions;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_PermissionsPage_MicrophoneButton : HoloKitAppUITemplate_PermissionsPage_PermissionButton
    {
        private void Awake()
        {
            PermissionsAPI.OnRequestMicrophonePermissionCompleted += OnRequestMicrophonePermissionCompleted;
        }

        private void OnDestroy()
        {
            PermissionsAPI.OnRequestMicrophonePermissionCompleted -= OnRequestMicrophonePermissionCompleted;
        }

        private void OnRequestMicrophonePermissionCompleted(bool granted)
        {
            if (granted)
            {
                UpdatePermissionButton(HoloKitAppPermissionStatus.Granted);
            }
            else
            {
                UpdatePermissionButton(HoloKitAppPermissionStatus.Denied);
            }
        }

        protected override HoloKitAppPermissionStatus GetPermissionStatus()
        {
            return HoloKitAppPermissionsManager.GetMicrophonePermissionStatus();
        }

        protected override void RequestPermission()
        {
            PermissionsAPI.RequestMicrophonePermission();
        }
    }
}
