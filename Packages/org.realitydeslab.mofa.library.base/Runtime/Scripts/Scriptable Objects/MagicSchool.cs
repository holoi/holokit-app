// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace RealityDesignLab.MOFA.Library.Base
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MagicSchool")]
    public class MagicSchool : ScriptableObject
    {
        public string TokenId;

        public string Name;
    }
}