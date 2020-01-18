using Asm;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class ColorChangeSlot : Draggable.Slot
    {
        [SerializeField]
        private Color dragOverColor = Color.white;

        private Color origColor;

        private Image image;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        public override void HandleDragEnter()
        {
            origColor = image.color;
            image.color = dragOverColor;
        }

        public override void HandleDragExit()
        {
            image.color = origColor;
        }
    }
}