// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using UnityEngine;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class PolyDragonController : MonoBehaviour
    {
        [SerializeField] private GameObject _fireBallPrefab;

        [SerializeField] private Transform _dragonMouse;

        private const float SpawnFireBallInterval = 3f;

        private const float FireBallSpeed = 100f;

        private void Start()
        {
            StartCoroutine(PeriodicFireBreath());
        }

        private IEnumerator PeriodicFireBreath()
        {
            while(true)
            {
                yield return new WaitForSeconds(SpawnFireBallInterval);
                SpawnFireBall();
            }
        }

        private void SpawnFireBall()
        {
            var fireBall = Instantiate(_fireBallPrefab, _dragonMouse);
            fireBall.GetComponent<AudioSource>().enabled = false;
            fireBall.GetComponent<Rigidbody>().AddForce(FireBallSpeed * _dragonMouse.forward);
            Destroy(fireBall, 3f);
        }

        public void PlaySound()
        {

        }
    }
}
