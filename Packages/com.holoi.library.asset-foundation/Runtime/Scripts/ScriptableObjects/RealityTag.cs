using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/RealityTag")]
    public class RealityTag : Tag 
    {
        public string BundleId;

        public string DisplayName;
    }
}