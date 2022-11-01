using UnityEngine;
using System.Collections;
using System;

namespace Holoi.Library.ARUX
{
    [RequireComponent(typeof(HoverableObject), typeof(Animator))]
    public class LoadButtonController : MonoBehaviour
    {
        public event Action OnDisableEvent;

        //animator
        Animator _animator;
        MeshRenderer _mr;
        Material _mat;

        // loading
        float _process;
        bool _isTriggered = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _mr = GetComponent<MeshRenderer>();
            _mat = _mr.material;
        }

        private void OnEnable()
        {
            // reset animations: 
            OnBirth();

            // reset properties: 
            _process = 0;
            _isTriggered = false;
        }

        private void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }

        void Update()
        {

            if(_process == 1 && !_isTriggered)
            {
                _isTriggered = true;
                OnDie();
            }
            else
            {

            }

            _process = GetComponent<HoverableObject>().Process;
            _mat.SetFloat("_Load", _process);
        }

        public void SetButtonTexture(Texture2D tex)
        {
            _mat.SetTexture("_Texture", tex);
        }

        public void OnDie()
        {
            _animator.SetTrigger("Die");
        }

        public void OnBirth()
        {
            _animator.Rebind();
            _animator.Update(0);
        }

        // animation events
        public void DisableAfterDieAnimation(AnimationEvent animationEvent)
        {
            gameObject.SetActive(false);
        }
    }
}