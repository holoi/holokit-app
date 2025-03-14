// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Holoi.Library.HoloKitAppLib.UI
{
    public class HoloKitAppUIComponent_TextButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text _text;

        [SerializeField] private Color _normalColor;

        [SerializeField] private Color _pressedColor;

        private void Start()
        {
            _text.color = _normalColor;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _text.color = _pressedColor;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _text.color = _normalColor;
        }
    }
}
