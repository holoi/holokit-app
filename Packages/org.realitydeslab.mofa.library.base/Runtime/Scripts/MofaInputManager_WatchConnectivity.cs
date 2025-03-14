// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitAppLib.WatchConnectivity;
using Holoi.Library.HoloKitAppLib.WatchConnectivity.MOFA;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.Library.Base
{
    /// <summary>
    /// This is the part of the MofaInputManager class controlling MOFA WatchConnectivity.
    /// </summary>
    public partial class MofaInputManager : MonoBehaviour
    {
        public MofaWatchState CurrentWatchState;

        private void InitializeWatchConnectivity()
        {
            MofaWatchConnectivityAPI.OnReceivedStartRoundMessage += OnReceivedRoundMessage;
            MofaWatchConnectivityAPI.OnWatchStateChanged += OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered += OnWatchTriggered;
            MofaWatchConnectivityAPI.OnReceivedHealthDataMessage += OnReceivedHealthDataMessage;
            MofaBaseRealityManager.OnMofaPhaseChanged += OnMofaPhaseChanged;

            HoloKitAppWatchConnectivityAPI.UpdateWatchPanel(HoloKitWatchPanel.MOFA);
            MofaWatchConnectivityAPI.Initialize();
        }

        private void DeinitializeWatchConnectivity()
        {
            MofaWatchConnectivityAPI.OnReceivedStartRoundMessage -= OnReceivedRoundMessage;
            MofaWatchConnectivityAPI.OnWatchStateChanged -= OnWatchStateChanged;
            MofaWatchConnectivityAPI.OnWatchTriggered -= OnWatchTriggered;
            MofaWatchConnectivityAPI.OnReceivedHealthDataMessage -= OnReceivedHealthDataMessage;
            MofaBaseRealityManager.OnMofaPhaseChanged -= OnMofaPhaseChanged;
        }

        private void OnReceivedRoundMessage()
        {
            if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Waiting)
                _mofaBaseRealityManager.TryGetReady();
            else if (_mofaBaseRealityManager.CurrentPhase.Value == MofaPhase.Fighting)
                MofaWatchConnectivityAPI.OnRoundStarted(int.Parse(HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId));
        }

        private void OnWatchStateChanged(MofaWatchState watchState)
        {
            CurrentWatchState = watchState;
        }

        private void OnWatchTriggered()
        {
            TryCastSpell();
        }

        private void OnReceivedHealthDataMessage(float distance, float energy)
        {
            Debug.Log($"[WatchConnectivity] Received health data from watch, distance: {distance} and energy: {energy}");
            var localMofaPlayer = _mofaBaseRealityManager.LocalMofaPlayer;
            localMofaPlayer.UpdateHealthDataServerRpc(distance, energy);
        }

        private void OnMofaPhaseChanged(MofaPhase newPhase)
        {
            if (HoloKitApp.Instance.IsSpectator)
                return;

            if (newPhase == MofaPhase.Countdown)
            {
                MofaWatchConnectivityAPI.OnRoundStarted(int.Parse(HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId));
                return;
            }

            //if (newPhase == MofaPhase.Fighting)
            //{
            //    MofaWatchConnectivityAPI.QueryWatchState();
            //    return;
            //}

            if (newPhase == MofaPhase.RoundResult)
            {
                var localPlayer = _mofaBaseRealityManager.LocalMofaPlayer;
                var localPlayerStats = _mofaBaseRealityManager.GetPlayerStats(localPlayer);
                MofaWatchConnectivityAPI.OnRoundEnded((int)localPlayerStats.PersonalRoundResult,
                                                           localPlayerStats.Kill,
                                                           localPlayerStats.HitRate);
                return;
            }
        }
    }
}
