using UnityEngine;
using UnityEngine.VFX;

namespace Holoi.Reality.MOFATheHunting
{
    public class FireBallController : MonoBehaviour
    {
        [SerializeField] private GameObject _fireBall;

        [SerializeField] private VisualEffect _explosionVfx;

        [SerializeField] private GameObject _pointLight;

        private const string TagName = "Plane";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagName))
            {
                var rigidbody = GetComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.velocity = Vector3.zero;
                _fireBall.SetActive(false);
                _pointLight.SetActive(false);
                _explosionVfx.SendEvent("OnExplode");
                Destroy(gameObject, 1.2f);
            }
        }
    }
}
