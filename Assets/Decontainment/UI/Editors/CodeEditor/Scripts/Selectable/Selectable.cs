using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Editor.Code
{
    public abstract class Selectable : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;

        private SelectionManager selectionManager;

        public bool Selected => selected;

        public void Init(SelectionManager selectionManager)
        {
            this.selectionManager = selectionManager;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) {
                return;
            }

            selectionManager.OnSelectableClicked(this, Input.GetKey(KeyCode.LeftShift));
        }

        public void OnDragStart(PointerEventData eventData)
        {
            selectionManager.OnSelectableDraggableDragged(eventData);
        }

        public void Select()
        {
            selected = true;
            SubSelect();
        }
        public void Deselect()
        {
            selected = false;
            SubDeselect();
        }

        protected abstract void SubSelect();
        protected abstract void SubDeselect();
    }
}