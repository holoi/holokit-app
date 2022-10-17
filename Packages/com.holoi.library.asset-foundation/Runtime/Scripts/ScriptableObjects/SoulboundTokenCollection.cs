using UnityEngine;
using Holoi.AssetFoundation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SoulboundTokenCollection")]
    public class SoulboundTokenCollection : NonFungibleCollection
    {
        public override List<NonFungible> NonFungibles
        {
            get
            {
                return SoulboundTokens.Cast<NonFungible>().ToList();
            }
        }

        public List<SoulboundToken> SoulboundTokens;

        public override NonFungible CoverNonFungible => CoverSoulboundToken;

        public SoulboundToken CoverSoulboundToken;

        public override List<Tag> Tags
        {
            get
            {
                return SoulboundTokenTags.Cast<Tag>().ToList();
            }
        }

        public List<SoulBoundTokenTag> SoulboundTokenTags;
    }
}