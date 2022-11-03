using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public enum LifeShieldArea
    {
        Center = 0,
        Top = 1,
        Left = 2,
        Right = 3
    }

    public class LifeShieldFragment : MonoBehaviour, IDamageable
    {
        [SerializeField] private LifeShieldArea _area;

        public LifeShieldArea Area => _area;

        public void OnDamaged(ulong attackerClientId)
        {
            GetComponentInParent<LifeShield>().OnDamaged(_area, attackerClientId);
        }
    }
}