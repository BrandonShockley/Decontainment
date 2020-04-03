using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
{
    public class ColorChangeSelectable : Selectable
    {
        [SerializeField]
        private Color selectColor = Color.white;

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
            image.color = selectColor;
        }

        public override void SubDeselect()
        {
            image.color = oldColor;
        }
    }
}