using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
{
    public class ColorDarkenSelectable : Selectable
    {
        [SerializeField]
        private Color darkenMultiplier = Color.white;

        private Color oldColor;

        private Image image;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        void Start()
        {
            oldColor = image.color;
        }

        public override void SubSelect()
        {
            image.color *= darkenMultiplier;
        }

        public override void SubDeselect()
        {
            image.color = oldColor;
        }
    }
}