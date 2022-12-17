using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheHunting
{
    public class TheDragonAttackTriggerController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!HoloKitApp.Instance.IsHost) { return; }

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                if (other.GetComponentInParent<NetworkObject>().OwnerClientId != 0)
                {
                    damageable.OnDamaged(0);
                }
            }
        }
    }
}
