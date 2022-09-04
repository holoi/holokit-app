using UnityEngine;
using UnityEngine.UI;

namespace StarUI
{
    public class ScrollBarHelper : MonoBehaviour
    {
        public float ScrollBarDefaultValue = 0f;
        [SerializeField] Image _background;
        [SerializeField] Image _extraBackground;

        public Image BackGround
        {
            get { return _background; }
        }

        public Image ExtraBackGround
        {
            get { return _extraBackground; }
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void EnableUILayout()
        {
            FindObjectOfType<StarUIManager>().EnableUILayout();
        }
    }
}


