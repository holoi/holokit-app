using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using HoloKit;

namespace Holoi.Library.HoloKitApp
{
    public class ARRayCastController : MonoBehaviour
    {

        [Header("Editor Mode")]
        public bool HitDebug = false;

        [Header("Events")]
        public UnityEvent HitEvent;

        public UnityEvent NotHitEvent;

        private Transform _centerEye;

        private ARRaycastManager _arRaycastManager;

        bool _isHit = false;


        void Start()
        {
            _centerEye = HoloKitCamera.Instance.CenterEyePose;
            _arRaycastManager = FindObjectOfType<ARRaycastManager>();
        }

        void Update()
        {
#if UNITY_IOS && !UNITY_EDITOR

        Vector3 horizontalForward = new Vector3(_centerEye.forward.x, 0, _centerEye.forward.z);
        Vector3 gazeOrientation = _centerEye.forward;

        var dot = Vector3.Dot(gazeOrientation, Vector3.down);
        var tilt = dot / 1;
        var aimPointDistance = 0f;
        if (tilt > 0)
        {
            aimPointDistance = MathHelpers.Remap(tilt, 1, 0, .05f, 5, true);
        }
        else
        {
            aimPointDistance = MathHelpers.Remap(tilt, 0, -1, 5, .05f, true);
        }

        Vector3 rayOrigin = _centerEye.position + horizontalForward.normalized * aimPointDistance;
        Ray ray = new(rayOrigin, Vector3.down);
        List<ARRaycastHit> hitResults = new();

        if (_arRaycastManager.Raycast(ray, hitResults, TrackableType.Planes))
        {
            foreach (var hitResult in hitResults)
            {
                var arPlane = hitResult.trackable.GetComponent<ARPlane>();
                if (arPlane.alignment == PlaneAlignment.HorizontalUp && arPlane.classification == PlaneClassification.Floor)
                {
                    transform.position = hitResult.pose.position;
                    _isHit = true;
                    HitEvent?.Invoke();
                    return;
                }
            }
            _isHit = false;
            NotHitEvent?.Invoke();
            transform.position = _centerEye.position + horizontalForward.normalized * 1.5f;

        }
        else
        {
            _isHit = false;
            NotHitEvent?.Invoke();
            transform.position = _centerEye.position + horizontalForward.normalized * 1.5f;
        }

#else

            if (HitDebug)
            {
                HitEvent?.Invoke();
            }
            else
            {
                transform.position = new Vector3(0, -1, 1.5f);
                NotHitEvent?.Invoke();
            }
#endif
        }
    }
}