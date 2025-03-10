// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.ARUX;
using HoloKit;

namespace RealityDesignLab.QuantumRealm
{
    public class QuantumRealmRealityManager : RealityManager
    {
        [Header("AR")]
        [SerializeField] private AROcclusionManager _arOcclusionManager;

        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementManager _arPlacementIndicator;

        [SerializeField] private HoverableStartButton _hoverableStartButton;

        [Header("Hand")]
        public Transform HostHandPoint;

        [Header("Apple")]
        public CoreHapticsManager CoreHapticsManager;

        [Header("Quantum Realm")]
        [SerializeField] private NetworkObject _buddhaGroupPrefab;

        private BuddhaGroup _buddhaGroup;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
                HoloKitHandTracker.Instance.IsActive = true;
                _arPlacementIndicator.IsActive = true;
            }
            else
            {
                // Delete unnecessary objects on client at the very beginning
                Destroy(_arPlacementIndicator.gameObject);
                Destroy(_hoverableStartButton.gameObject);
            }
        }

        public void OnSessionStarted()
        {
            // Spawn the buddha group
            SpawnBuddhaGroup();

            // Turn off indicators and buttons
            _arPlacementIndicator.OnPlacedFunc();
            _hoverableStartButton.OnDeath();

            // Turn off plane detection and raycast
            HoloKitApp.Instance.ARSessionManager.SetARPlaneManagerEnabled(false);
            HoloKitApp.Instance.ARSessionManager.SetARRaycastManagerEnabled(false);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                HoloKitApp.Instance.ARSessionManager.SetEnvironmentOcclusionEnabled(false);
        }

        private void SpawnBuddhaGroup()
        {
            var hitPoint = _arPlacementIndicator.HitPoint;
            Vector3 position = new(HoloKitCameraManager.Instance.CenterEyePose.transform.position.x,
                                   hitPoint.position.y,
                                   HoloKitCameraManager.Instance.CenterEyePose.transform.position.z);
            var buddhaGroup = Instantiate(_buddhaGroupPrefab, position, hitPoint.rotation * Quaternion.Euler(180f * Vector3.up));
            buddhaGroup.Spawn();
        }

        public void SetBuddhaGroup(BuddhaGroup buddhaGroup)
        {
            _buddhaGroup = buddhaGroup;
        }
    }
}