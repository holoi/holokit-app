// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using HoloKit;

namespace Holoi.Library.HoloKitAppLib
{
    [DisallowMultipleComponent]
    public class HoloKitAppARSessionManager : MonoBehaviour
    {
        [Header("Image Tracking")]
        [SerializeField] private XRReferenceImageLibrary _holokitAppReferenceImageLibrary;

        [SerializeField] private GameObject _trackedImagePrefab;

        [Header("Plane Detection")]
        [SerializeField] private ARPlane _arPlanePrefab;

        public ARTrackedImageManager ARTrackedImageManager => _arTrackedImageManager;

        public ARPlaneManager ARPlaneManager => _arPlaneManager;

        public ARRaycastManager ARRaycastManager => _arRaycastManager;

        public AROcclusionManager AROcclusionManager => _arOcclusionManager;

        private ARTrackedImageManager _arTrackedImageManager;

        private ARPlaneManager _arPlaneManager;

        private ARRaycastManager _arRaycastManager;

        private AROcclusionManager _arOcclusionManager;

        private bool _humanOcclusionEnabled;

        private void Start()
        {
            var xrOrigin = HoloKitCameraManager.Instance.GetComponentInParent<XROrigin>();

            // Setup Image Tracking for space anchor sharing
            if (xrOrigin.TryGetComponent(out _arTrackedImageManager))
            {
                // Host does not need image tracker
                if (HoloKitApp.Instance.IsHost)
                {
                    Destroy(_arTrackedImageManager);
                }
                else
                {
                    SetupARTrackedImageManager();
                }
            }
            else
            {
                // Add ARTrackedImageManager if client does not have one
                if (!HoloKitApp.Instance.IsHost)
                {
                    _arTrackedImageManager = xrOrigin.gameObject.AddComponent<ARTrackedImageManager>();
                    SetupARTrackedImageManager();
                }
            }

            // Open default human occlusion on spectators
            if (HoloKitApp.Instance.IsSpectator)
            {
                SetHumanOcclusionEnabled(true);
            }

            // Register callbacks
            HoloKitCameraManager.OnHoloKitRenderModeChanged += OnHoloKitRenderModeChanged;
        }

        private void OnDestroy()
        {
            HoloKitCameraManager.OnHoloKitRenderModeChanged -= OnHoloKitRenderModeChanged;
        }

        private void SetupARTrackedImageManager()
        {
            _arTrackedImageManager.enabled = false;
            _arTrackedImageManager.referenceLibrary = _holokitAppReferenceImageLibrary;
            _arTrackedImageManager.requestedMaxNumberOfMovingImages = 1;
            _arTrackedImageManager.trackedImagePrefab = _trackedImagePrefab;
        }

        public void SetARTrackedImageManagerEnabled(bool enabled)
        {
            if (_arTrackedImageManager == null) { return; }

            foreach (var trackable in _arTrackedImageManager.trackables)
            {
                trackable.gameObject.SetActive(enabled);
            }
            _arTrackedImageManager.enabled = enabled;
        }

        public void AddARPlaneManager()
        {
            var xrOrigin = HoloKitCameraManager.Instance.GetComponentInParent<XROrigin>();

            if (xrOrigin.TryGetComponent<ARPlaneManager>(out var _))
            {
                Debug.Log("[HoloKitAppARSessionManager] ARPlaneManager already added");
                return;
            }

            _arPlaneManager = xrOrigin.gameObject.AddComponent<ARPlaneManager>();
            _arPlaneManager.planePrefab = _arPlanePrefab.gameObject;
            _arPlaneManager.enabled = true;
        }

        public void SetARPlaneManagerEnabled(bool enabled)
        {
            if (_arPlaneManager == null)
            {
                var xrOrigin = HoloKitCameraManager.Instance.GetComponentInParent<XROrigin>();
                if (!xrOrigin.TryGetComponent<ARPlaneManager>(out _arPlaneManager))
                {
                    return;
                }
            }
            _arPlaneManager.enabled = enabled;
            if (!enabled)
            {
                // Destory all detected ARPlane objects
                DestroyExistingARPlanes();
            }
        }

        private void DestroyExistingARPlanes()
        {
            var planes = FindObjectsByType<ARPlane>(FindObjectsSortMode.None);
            foreach (var plane in planes)
            {
                Destroy(plane.gameObject);
            }
        }

        public void AddARRaycastManager()
        {
            var xrOrigin = HoloKitCameraManager.Instance.GetComponentInParent<XROrigin>();

            if (xrOrigin.TryGetComponent<ARRaycastManager>(out var _))
            {
                Debug.Log("[HoloKitAppARSessionManager] ARRaycastManager already added");
                return;
            }

            _arRaycastManager = xrOrigin.gameObject.AddComponent<ARRaycastManager>();
            _arRaycastManager.enabled = true;
        }

        public void SetARRaycastManagerEnabled(bool enabled)
        {
            if (_arRaycastManager == null)
            {
                var xrOrigin = HoloKitCameraManager.Instance.GetComponentInParent<XROrigin>();
                if (!xrOrigin.TryGetComponent(out _arRaycastManager))
                {
                    return;
                }
            }
            _arRaycastManager.enabled = enabled;
        }

        private void AddAROcclusionManager()
        {
            _arOcclusionManager = HoloKitCameraManager.Instance.gameObject.AddComponent<AROcclusionManager>();
            _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Disabled;
            _arOcclusionManager.environmentDepthTemporalSmoothingRequested = false;
            _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Disabled;
            _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
            _arOcclusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferEnvironmentOcclusion;
        }

        public bool GetHumanOcclusionEnabled()
        {
            if (_arOcclusionManager == null)
            {
                _arOcclusionManager = HoloKitCameraManager.Instance.GetComponent<AROcclusionManager>();
                if (_arOcclusionManager == null)
                {
                    return false;
                }
            }
            if (_arOcclusionManager.requestedHumanDepthMode != HumanSegmentationDepthMode.Disabled
                && _arOcclusionManager.requestedHumanStencilMode != HumanSegmentationStencilMode.Disabled
                && _arOcclusionManager.requestedOcclusionPreferenceMode != OcclusionPreferenceMode.NoOcclusion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetHumanOcclusionEnabled(bool enabled)
        {
            if (_arOcclusionManager == null)
            {
                _arOcclusionManager = HoloKitCameraManager.Instance.GetComponent<AROcclusionManager>();
                if (_arOcclusionManager == null)
                {
                    AddAROcclusionManager();
                }
            }

            if (enabled)
            {
                if (!_arOcclusionManager.enabled)
                {
                    _arOcclusionManager.enabled = true;
                }
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Fastest;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Fastest;
                //_arOcclusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;
            }
            else
            {
                _arOcclusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Disabled;
                _arOcclusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
                //_arOcclusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.NoOcclusion;
            }
        }

        public void SetEnvironmentOcclusionEnabled(bool enabled)
        {
            if (_arOcclusionManager == null)
            {
                _arOcclusionManager = HoloKitCameraManager.Instance.GetComponent<AROcclusionManager>();
                if (_arOcclusionManager == null)
                    return;
            }

            if (enabled)
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Fastest;
            }
            else
            {
                _arOcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Disabled;
            }
        }

        //private void Update()
        //{
        //    if (_arOcclusionManager)
        //    {
        //        Debug.Log($"Occlusion status: {_arOcclusionManager.enabled} {_arOcclusionManager.requestedHumanStencilMode} {_arOcclusionManager.requestedHumanDepthMode} {_arOcclusionManager.requestedOcclusionPreferenceMode}");
        //        return;
        //    }

        //    _arOcclusionManager = FindObjectOfType<AROcclusionManager>();
        //    if (_arOcclusionManager)
        //    {
        //        Debug.Log($"Occlusion status: {_arOcclusionManager.enabled} {_arOcclusionManager.requestedHumanStencilMode} {_arOcclusionManager.requestedHumanDepthMode} {_arOcclusionManager.requestedOcclusionPreferenceMode}");
        //    }
        //}

        private void OnHoloKitRenderModeChanged(HoloKitRenderMode renderMode)
        {
            if (renderMode == HoloKitRenderMode.Stereo)
            {
                _humanOcclusionEnabled = GetHumanOcclusionEnabled();
                SetHumanOcclusionEnabled(false);
            }
            else if (renderMode == HoloKitRenderMode.Mono)
            {
                if (_humanOcclusionEnabled)
                {
                    SetHumanOcclusionEnabled(true);
                }
            }
        }
    }
}
