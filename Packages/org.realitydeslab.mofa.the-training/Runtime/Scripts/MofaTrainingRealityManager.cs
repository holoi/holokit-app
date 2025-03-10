// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;
using RealityDesignLab.MOFA.Library.Base;
using Holoi.Library.ARUX;

namespace RealityDesignLab.MOFA.TheTraining
{
    public class MofaTrainingRealityManager : MofaBaseRealityManager
    {
        [Header("MOFA The Training")]
        [SerializeField] private MofaAIPlayer _mofaAIPlayerPrefab;

        [Header("AR")]
        [SerializeField] private ARPlaneManager _arPlaneManager;

        [SerializeField] private ARRaycastManager _arRaycastManager;

        [SerializeField] private ARPlacementManager _arPlacementManager;

        [SerializeField] private MofaARPlacementIndicator _arPlacementIndicator;

        private MofaAIPlayer _mofaAIPlayer;

        /// <summary>
        /// This event is called when the player tries to start the game in an invalid position.
        /// </summary>
        public static event Action OnFailedToStartAtCurrentPosition;

        private void Start()
        {
            if (HoloKitApp.Instance.IsHost)
            {
                _arPlaneManager.enabled = true;
                _arRaycastManager.enabled = true;
                _arPlacementManager.IsActive = true;
            }
            else
            {
                Destroy(_arPlacementManager.gameObject);
                Destroy(_arPlacementIndicator.gameObject);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
                SpawnMofaAIPlayer();
        }

        private void SpawnMofaAIPlayer()
        {
            _mofaAIPlayer = Instantiate(_mofaAIPlayerPrefab);
            _mofaAIPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(MofaAIPlayer.AIClientId);
        }

        public override void TryGetReady()
        {
            // We only need to select a proper spawn point in the first round
            if (RoundCount.Value == 1)
            {
                if (_arPlacementManager.IsValid)
                {
                    GetReady();
                }
                else
                {
                    OnFailedToStartAtCurrentPosition?.Invoke();
                }
            }
            else
            {
                GetReady();
            }
        }

        protected override void SetupRound()
        {
            if (RoundCount.Value == 1)
            {
                // Spawn the avatar
                Vector3 position = _arPlacementManager.HitPoint.position;
                Quaternion rotation = _arPlacementManager.HitPoint.rotation;
                var realityBundleId = HoloKitApp.Instance.CurrentReality.BundleId;
                var realityPreferences = HoloKitApp.Instance.GlobalSettings.RealityPreferences[realityBundleId];
                _mofaAIPlayer.SpawnAvatarClientRpc(realityPreferences.MetaAvatarCollectionBundleId, realityPreferences.MetaAvatarTokenId, position, rotation);
                // Turn off ARPlacementManager, ARPlaneManager and ARRaycastManager
                _arPlacementManager.OnPlacedFunc();
                _arPlaneManager.enabled = false;
                _arRaycastManager.enabled = false;
            }

            base.SetupRound();
        }
    }
}
