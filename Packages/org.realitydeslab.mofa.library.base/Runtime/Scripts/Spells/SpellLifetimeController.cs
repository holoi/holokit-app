// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Netcode;

namespace RealityDesignLab.MOFA.Library.Base
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(AudioSource))]
    public class SpellLifetimeController : NetworkBehaviour
    {
        [SerializeField] private float _lifetime;

        [SerializeField] private AudioClip _spawnSound;

        [SerializeField] private float _destroyDelay;

        private float _timeElapsed;

        private bool _isAlive;

        public event Action OnLifetimeEnded;

        private void OnEnable()
        {
            _isAlive = true;
            _timeElapsed = 0f;
            PlaySpawnSound();
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                _timeElapsed += Time.fixedDeltaTime;
                if (_isAlive)
                {
                    if (_timeElapsed > _lifetime)
                    {
                        _isAlive = false;
                        OnLifetimeEndedClientRpc();
                    }
                }
                else
                {
                    if (_timeElapsed > _lifetime + _destroyDelay)
                    {
                        GetComponent<NetworkObject>().Despawn();
                    }
                }
            }
        }

        private void PlaySpawnSound()
        {
            if (_spawnSound != null)
            {
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = _spawnSound;
                audioSource.Play();
            }
        }

        [ClientRpc]
        private void OnLifetimeEndedClientRpc()
        {
            OnLifetimeEnded?.Invoke();
        }
    }
}