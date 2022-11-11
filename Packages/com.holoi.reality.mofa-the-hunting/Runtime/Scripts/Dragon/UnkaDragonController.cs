using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Animations.Rigging;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;
using Holoi.Library.MOFABase;
using Holoi.Library.ARUX;
using HoloKit;

namespace Holoi.Reality.MOFATheHunting
{
    public class UnkaDragonController : NetworkBehaviour
    {
        [SerializeField] private MultiAimConstraint _headAimConstraint;

        [SerializeField] private MultiAimConstraint _chestAimConstraint;

        [SerializeField] private RigBuilder _rigBuilder;

        private bool _isAttacking;

        [Header("Attack Behaviour")]
        [SerializeField] Animator _aniamtor;

        [SerializeField] GameObject _fireBallPrefab;

        [SerializeField] GameObject _fireBreathPrefab;

        [SerializeField] Transform _powerInitPoint;

        GameObject _fireBallInstance;

        GameObject _fireBreathInstance;

        Transform _attackTarget;

        private Animator _headTargetAnimator;

        [Header("Renderer")]
        [SerializeField] SkinnedMeshRenderer _dragonRenderer;

        [SerializeField] SkinnedMeshRenderer _eyeRenderer;

        [SerializeField] VisualEffect _dragonDeathVFX;

        public Vector3 ClipPlane
        {
            get => _clipPlane;
            set
            {
                _clipPlane = value;
            }
        }

        public float ClipPlaneHeihgt
        {
            get => _clipPlaneHeight;
            set
            {
                _clipPlaneHeight = value;
            }
        }

        private Vector3 _clipPlane = new(2f, 0f, -1f);

        private float _clipPlaneHeight = 3f;

        bool _isDuringDeath = false;

        [Header("Test")]
        public GameObject DeathVFX;

        public bool Reset;

        public bool Die;

        public bool FireBall;

        public bool FireBreath;

        public bool AutoFireBall;

        public bool AutoFireBreath;

        private void Start()
        {
            //if (AutoFireBall)
            //    StartCoroutine(WaitAndFireBall());
            //if (AutoFireBreath)
            //    StartCoroutine(WaitAndFireBreath());

            _clipPlane = -transform.forward;
            _clipPlaneHeight = (transform.position + 2f * transform.forward).magnitude;
            SetRenderClip();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SetupHeadTarget();
            if (IsServer)
            {
                StartInitialMovement();
            }
        }

        private void SetupHeadTarget()
        {
            // Get host head target
            Transform headTarget = ((MofaHuntingRealityManager)HoloKitApp.Instance.RealityManager).HeadTarget;
            // Set head aim
            var headData = _headAimConstraint.data.sourceObjects;
            headData.SetTransform(0, headTarget);
            _headAimConstraint.data.sourceObjects = headData;
            // Set chest aim
            var chestData = _chestAimConstraint.data.sourceObjects;
            chestData.SetTransform(0, headTarget);
            _chestAimConstraint.data.sourceObjects = chestData;
            // Rebuild
            _rigBuilder.Build();
            _headTargetAnimator = headTarget.GetComponent<Animator>();
        }

        private void StartInitialMovement()
        {
            LeanTween.move(gameObject, transform.position + 4f * transform.forward, 5f)
                .setOnComplete(() =>
                {
                    _isAttacking = true;
                });
        }

        private void Update()
        {

            //if (Die)
            //{
            //    _aniamtor.SetTrigger("Die");
            //    Die = false;
            //    OnDeath();
            //}

            //if (FireBall)
            //{
            //    _aniamtor.SetTrigger("Fire Ball");
            //    FireBall = false;
            //}


            //if (FireBreath)
            //{
            //    _aniamtor.SetTrigger("Fire Breath");
            //    _targetAnimator.SetTrigger("Fire Breath");
            //    FireBreath = false;
            //}

            //if (Reset)
            //{
            //    _clipPlaneHeight = 3f;

            //    var plane = new Vector4(_clipPlane.x, _clipPlane.y, _clipPlane.z, _clipPlaneHeight);

            //    foreach (var material in _dragonRenderer.materials)
            //    {
            //        material.SetVector("_Clip_Plane", plane);
            //    }

            //    _eyeRenderer.material.SetVector("_Clip_Plane", plane);

            //    _dragonDeathVFX.SetVector4("Clip Plane", plane);

            //    _aniamtor.Rebind();
            //    _aniamtor.Update(0);
            //    Reset = false;
            //}

            //if (_isDuringDeath)
            //{
            //    UpdateRendererClipPlaneDuraingDeathAnimation();
            //}

            if (true)
            {
                SetRenderClip();
            }
        }

        void UpdateRendererClipPlaneDuraingDeathAnimation()
        {
            _clipPlaneHeight -= Time.deltaTime * 3f;

            if (_clipPlaneHeight < -3f)
            {
                _clipPlaneHeight = -3f;
                _isDuringDeath = false;
            }
        }

        private void SetRenderClip()
        {
            var pos = _clipPlane;
            var plane = new Vector4(pos.x, pos.y, pos.z, _clipPlaneHeight);

            foreach (var material in _dragonRenderer.materials)
            {
                material.SetVector("_Clip_Plane", plane);
            }

            _eyeRenderer.material.SetVector("_Clip_Plane", plane);

            _dragonDeathVFX.SetVector4("Clip Plane", plane);
        }

        public void OnFireBallInit()
        {
            _fireBallInstance = Instantiate(_fireBallPrefab);
            _fireBallInstance.GetComponent<FireBreathController>().followPoint = _powerInitPoint;
        }

        public void OnFireBallAttack()
        {
            _fireBallInstance.GetComponent<FireBreathController>().IsFollow = false;
            var dir = (_attackTarget == null ? new Vector3(0, -2, 2) : _attackTarget.position - _powerInitPoint.position).normalized;
            var speed = dir * 3f;
            _fireBallInstance.GetComponent<Rigidbody>().velocity = speed;
        }

        public void OnFireBreathInit()
        {
            Debug.Log("OnFireBreathInit");
            _fireBreathInstance = Instantiate(_fireBreathPrefab);
            _fireBreathInstance.GetComponent<FireBreathController>().followPoint = _powerInitPoint;
        }

        public void OnFireBreathAttack()
        {
            _fireBreathInstance.GetComponent<FireBreathController>().OnAttack();
        }

        //private void OnDrawGizmos()
        //{
        //    var dir = (_enemyPoint == null ? new Vector3(0, -2, 2) : _enemyPoint.position - _powerInitPoint.position).normalized;
        //    Debug.DrawRay(_powerInitPoint.position, dir*10f, Color.red);
        //}

        public void OnDeath()
        {
            _isDuringDeath = true;
        }

        public void OnAnimationStop()
        {
            _aniamtor.StopPlayback();
        }

        IEnumerator WaitAndDisableGameObject(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }

        public void PlaySound()
        {

        }

        IEnumerator WaitAndFireBall()
        {
            FireBall = true;
            yield return new WaitForSeconds(4.5f);
            StartCoroutine(WaitAndFireBall());
        }

        IEnumerator WaitAndFireBreath()
        {
            FireBreath = true;
            yield return new WaitForSeconds(10f);
            StartCoroutine(WaitAndFireBreath());
        }
    }
}