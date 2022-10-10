using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Holoi.Library.HoloKitApp
{
    public abstract class RealityManager : NetworkBehaviour
    {
        public List<GameObject> NetworkPrefabs;

        public List<UI.HoloKitAppUIPanel> UIPanelPrefabs;

        [SerializeField] private UniversalRenderPipelineAsset _urpAsset;

        public void SetupURPAsset()
        {
            if (_urpAsset != null)
            {
                GraphicsSettings.renderPipelineAsset = _urpAsset;
            }
            else
            {
                Debug.Log("[RealityManager] Current reality does not contain a customized URP Asset");
            }
        }
    }
}