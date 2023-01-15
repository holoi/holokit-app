using UnityEngine;
using Unity.Netcode;
using HoloKit;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheGhost
{
    public class Ghost : NetworkBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 8;

        public NetworkVariable<int> CurrentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private CharacterController _characterController;

        private float _movementSpeed = 0.005f;

        private void Awake()
        {
            UI.MofaGhostJoystickController.OnAxisChanged += OnAxisChanged;

            _characterController = GetComponent<CharacterController>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                CurrentHealth.Value = _maxHealth;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            UI.MofaGhostJoystickController.OnAxisChanged -= OnAxisChanged;
        }

        /// <summary>
        /// Called when the ghost player is inputting.
        /// </summary>
        /// <param name="axis"></param>
        private void OnAxisChanged(Vector2 axis)
        {
            Transform centerEyePose = HoloKitCamera.Instance.CenterEyePose;
            Vector3 horizontalForward = Vector3.ProjectOnPlane(centerEyePose.forward, Vector3.up);
            Vector3 horizontalRight = Vector3.ProjectOnPlane(centerEyePose.right, Vector3.up);

            Vector3 motion = _movementSpeed * (axis.y * horizontalForward + axis.x * horizontalRight);
            // Make the ghost heading to the movement direction
            transform.rotation = Quaternion.LookRotation(motion.normalized);
            // Move the ghost
            _characterController.Move(motion);
        }

        /// <summary>
        /// This method is called by the detection wave when the ghost is detected.
        /// </summary>
        [ClientRpc]
        public void OnDetectedClientRpc()
        {
            Debug.Log("[Ghost] On detected");
            if (HoloKitApp.Instance.IsPuppeteer)
            {
                // TODO: The puppeteer should know its ghost has been detected
            }
            else
            {
                // Both the observer and the attacker should see the ghost for a period of time
                OnBeingRevealed();
            }
        }

        private void OnBeingRevealed()
        {

        }

        public void OnDamaged(ulong attackerClientId)
        {
            OnDamagedClientRpc();

            CurrentHealth.Value--;
            if (CurrentHealth.Value == 0)
            {
                Debug.Log("Ghost is dead");
            }
        }

        [ClientRpc]
        private void OnDamagedClientRpc()
        {
            Debug.Log("[Ghost] On damaged");
            OnBeingRevealed();
            OnBeingHit();
        }

        private void OnBeingHit()
        {

        }
    }
}
