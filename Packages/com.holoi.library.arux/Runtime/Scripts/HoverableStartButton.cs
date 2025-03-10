// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using Holoi.Library.HoloKitAppLib;

namespace Holoi.Library.ARUX
{
    [RequireComponent(typeof(HoverableObject), typeof(Animator))]
    public class HoverableStartButton : MonoBehaviour
    {
        private Animator _animator;

        private MeshRenderer _meshRenderer;

        private HoverableObject _hoverableObject;

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _hoverableObject = GetComponent<HoverableObject>();
        }

        private void Update()
        {
            _meshRenderer.material.SetFloat("_Load", _hoverableObject.CurrentLoadPercentage);
        }

        public void OnAppear()
        {
            gameObject.SetActive(true);
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.Rebind();
            _animator.Update(0);
        }

        public void OnDisappear()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.SetTrigger("Die");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
                gameObject.SetActive(false);
            }));
        }

        public void OnDeath()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.SetTrigger("Die");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
                Destroy(gameObject);
            }));
        }
    }
}