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

        protected override void SubSelect()
        {
            image.color *= darkenMultiplier;
        }

        protected override void SubDeselect()
        {
            image.color = oldColor;
        }
    }
}