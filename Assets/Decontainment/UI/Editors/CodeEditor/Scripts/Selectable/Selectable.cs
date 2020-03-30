using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Editor.Code
{
    public abstract class Selectable : MonoBehaviour, IPointerClickHandler
    {
        private SelectionManager selectionManager;

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

        public abstract void Select();
        public abstract void Deselect();
    }
}