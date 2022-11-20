using System;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
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

        public event Action OnHit;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                GetComponent<Collider>().enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                var mofaRealityManager = HoloKitApp.HoloKitApp.Instance.RealityManager as MofaBaseRealityManager;
                ulong victimClientId = other.GetComponentInParent<NetworkObject>().OwnerClientId;
                if (mofaRealityManager.Players.ContainsKey(victimClientId))
                {
                    MofaTeam attackerTeam = mofaRealityManager.Players[OwnerClientId].Team.Value;
                    MofaTeam victimTeam = mofaRealityManager.Players[victimClientId].Team.Value;
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
        }

        private void OnHitFunc()
        {
            OnHitClientRpc();
            if (_hitOnce)
            {
                GetComponent<Collider>().enabled = false;
                Destroy(gameObject, _destroyDelay);
            }
        }

        [ClientRpc]
        private void OnHitClientRpc()
        {
            if (_hitOnce)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
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