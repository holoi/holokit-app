// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitAppLib;

namespace RealityDesignLab.MOFA.Library.Base
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Collider))]
    public class AttackSpell : NetworkBehaviour
    {
        [Tooltip("Will the spell explode after its first hit?")]
        [SerializeField] private bool _hitOnce = true;

        [SerializeField] private AudioClip _hitSound;

        [SerializeField] private float _destroyDelay;

        public bool HitOnce => _hitOnce;

        public event Action OnHit;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                GetComponent<Collider>().enabled = true;
            }
            else
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // TEST: Two attack spells can collide
            if (other.TryGetComponent<AttackSpell>(out var _))
            {
                var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                if (mofaBaseRealityManager != null)
                {
                    ulong victimClientId = other.GetComponent<NetworkObject>().OwnerClientId;
                    var mofaPlayerDict = mofaBaseRealityManager.MofaPlayerDict;
                    if (mofaPlayerDict.ContainsKey(victimClientId))
                    {
                        MofaTeam attackerTeam = mofaPlayerDict[OwnerClientId].Team.Value;
                        MofaTeam victimTeam = mofaPlayerDict[victimClientId].Team.Value;
                        if (attackerTeam != victimTeam)
                        {
                            OnHitFunc();
                        }
                    }
                }
            }

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                // For MOFA spell test scene
                if (HoloKitApp.Instance == null)
                {
                    damageable.OnDamaged(OwnerClientId);
                    OnHitFunc();
                    return;
                }

                var mofaBaseRealityManager = HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                if (mofaBaseRealityManager != null)
                {
                    ulong victimClientId = other.GetComponentInParent<NetworkObject>().OwnerClientId;
                    var mofaPlayerDict = mofaBaseRealityManager.MofaPlayerDict;
                    if (mofaPlayerDict.ContainsKey(victimClientId))
                    {
                        MofaTeam attackerTeam = mofaPlayerDict[OwnerClientId].Team.Value;
                        MofaTeam victimTeam = mofaPlayerDict[victimClientId].Team.Value;
                        if (attackerTeam != victimTeam)
                        {
                            damageable.OnDamaged(OwnerClientId);
                            OnHitFunc();
                        }
                    }
                    else
                    {
                        // The victim is not a player
                        damageable.OnDamaged(OwnerClientId);
                        OnHitFunc();
                    }
                }
                else // MOFA: The Ducks
                {
                    damageable.OnDamaged(OwnerClientId);
                    OnHitFunc();
                }
            }
        }

        // Host only
        private void OnHitFunc()
        {
            OnHitClientRpc();
            if (_hitOnce)
            {
                GetComponent<Collider>().enabled = false;
                StartCoroutine(HoloKitAppUtils.WaitAndDo(_destroyDelay, () =>
                {
                    GetComponent<NetworkObject>().Despawn();
                }));
            }
        }

        [ClientRpc]
        private void OnHitClientRpc()
        {
            if (_hitOnce)
            {
                GetComponent<SpellUniformLinearMotionController>().IsMoving = false;
            }
            OnHit?.Invoke();
            PlayHitSound();
        }

        private void PlayHitSound()
        {
            if (_hitSound != null)
            {
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = _hitSound;
                audioSource.Play();
            }
        }
    }
}