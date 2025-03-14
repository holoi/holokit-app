// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Netcode;
using RealityDesignLab.MOFA.Library.Base;

namespace RealityDesignLab.MOFA.TheDucks
{
    public class Duck : NetworkBehaviour, IDamageable
    {
        /// <summary>
        /// The initial force added to the rigibody when spawned.
        /// </summary>
        [SerializeField] private float _initialForce = 300f;

        /// <summary>
        /// The lifetime duration of the duck.
        /// </summary>
        [SerializeField] private float _lifetime = 4;

        /// <summary>
        /// The interval before destroying the gameObject after natural death.
        /// </summary>
        [SerializeField] private float _naturalDeathDelay = 1f;

        /// <summary>
        /// The interval before destroying the gameObject after being hit.
        /// </summary>
        [SerializeField] private float _beingHitDelay = 1f;

        private float _lifetimeElapsed = 0f;

        /// <summary>
        /// Invoked when the duck dies naturally.
        /// </summary>
        public event Action OnNaturalDeath;

        /// <summary>
        /// Invoked when the duck is hit by an attack spell.
        /// </summary>
        public event Action OnBeingHit;

        public override void OnNetworkSpawn()
        {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(_initialForce * transform.forward);
        }

        private void Update()
        {
            if (IsServer)
            {
                _lifetimeElapsed += Time.deltaTime;
                if (_lifetimeElapsed > _lifetime)
                {
                    OnNaturalDeathClientRpc();
                    Destroy(gameObject, _naturalDeathDelay);
                }
            }
        }

        [ClientRpc]
        private void OnNaturalDeathClientRpc()
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            OnNaturalDeath?.Invoke();
        }

        /// <summary>
        /// This method is called when the duck is hit by an attack spell.
        /// </summary>
        /// <param name="attackerClientId"></param>
        public void OnDamaged(ulong attackerClientId)
        {
            OnBeingHitClientRpc();
            Destroy(gameObject, _beingHitDelay);
        }

        [ClientRpc]
        private void OnBeingHitClientRpc()
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            OnBeingHit?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            if (other.CompareTag("Plane"))
            {
                OnDamaged(0);
            }
        }
    }
}
