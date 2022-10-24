using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Library.MOFABase
{
    public class ScreenEffectController : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;

        private void OnEnable()
        {
            LifeShield.OnTopDestroyed += OnBeingHit;
            LifeShield.OnRightDestroyed += OnBeingHit;
            LifeShield.OnLeftDestroyed += OnBeingHit;
            LifeShield.OnCenterDestroyed += OnBeingHit;
        }

        private void OnDisable()
        {
            LifeShield.OnTopDestroyed -= OnBeingHit;
            LifeShield.OnRightDestroyed -= OnBeingHit;
            LifeShield.OnLeftDestroyed -= OnBeingHit;
            LifeShield.OnCenterDestroyed -= OnBeingHit;
        }

        private void Start()
        {
            if(vfx == null) vfx = GetComponent<VisualEffect>();
        }

        public void OnBeingHit(ulong id)
        {
            vfx.SendEvent("OnBeingHit");
        }
    }
}
