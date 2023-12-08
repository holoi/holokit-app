// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIRealitySettingTab_Display : HoloKitAppUIRealitySettingTab
    {
        public override string TabName => "Display";

        [SerializeField] private TMP_Text _occlusionStatus;

        [SerializeField] private HoloKitAppUIComponent_AnimatedToggle _occlusionToggle;

        private void Start()
        {
            if (HoloKitApp.Instance.ARSessionManager.GetHumanOcclusionEnabled())
            {
                _occlusionToggle.Toggled = true;
                _occlusionStatus.text = "On";
            }
        }

        public void OnOcclusionToggleValueChanged(bool toggled)
        {
            if (toggled)
            {
                _occlusionStatus.text = "On";
                HoloKitApp.Instance.ARSessionManager.SetHumanOcclusionEnabled(true);
            }
            else
            {
                _occlusionStatus.text = "Off";
                HoloKitApp.Instance.ARSessionManager.SetHumanOcclusionEnabled(false);
            }
        }
    }
}
