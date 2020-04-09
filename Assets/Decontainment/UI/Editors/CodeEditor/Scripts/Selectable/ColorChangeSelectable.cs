using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
{
    public class ColorChangeSelectable : Selectable
    {
        [SerializeField]
        private Color selectColor = Color.white;
        [SerializeField]
        private Image image = null;

        private Color oldColor;

        void Awake()
        {
            if (image == null) {
                image = GetComponent<Image>();
            }
        }

        void Start()
        {
            oldColor = image.color;
        }

        protected override void SubSelect()
        {
            image.color = selectColor;
        }

        protected override void SubDeselect()
        {
            image.color = oldColor;
        }
    }
}