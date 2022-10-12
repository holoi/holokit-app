using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.QuantumBuddhas
{
    public class QuantumBuddhasSceneManager : MonoBehaviour
    {
        [SerializeField] List<VisualEffect> _vfxs = new List<VisualEffect>();
        [SerializeField] 

        int _amount = 0;
        int _index = 0;
        Animator _animator;

        void Start()
        {
            _amount = _vfxs.Count;
        }

        void Update()
        {
        
        }

        public void SwitchToNextVFX()
        {
            if(_animator !=null)
            {
                _animator.SetTrigger("Fade Out");
            }

            _index++;
            if (_index == _vfxs.Count) _index = 0;
            // disbale all vfx
            foreach (var vfx in _vfxs)
            {
                vfx.gameObject.SetActive(false);
            }
            // enable the slected
            _vfxs[_index].gameObject.SetActive(true);
            _animator = _vfxs[_index].GetComponent<Animator>();
            _animator.Play("Fade in", -1, 0f); // reset animator
        }
    }
}