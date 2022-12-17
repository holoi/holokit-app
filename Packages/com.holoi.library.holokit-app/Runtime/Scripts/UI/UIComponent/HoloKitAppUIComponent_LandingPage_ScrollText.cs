using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_LandingPage_ScrollText : MonoBehaviour
    {
        [SerializeField] private RectTransform _text;

        private const float MovementSpeed = 120f;

        private void Update()
        {
            _text.anchoredPosition += new Vector2(MovementSpeed * Time.deltaTime, 0f);
        }
    }
}
