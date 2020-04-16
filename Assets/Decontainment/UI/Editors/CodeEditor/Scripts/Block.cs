using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
{
    public abstract class Block : MonoBehaviour
    {
        [SerializeField]
        protected float collapsedWidth = 44.0f;

        protected CodeList codeList;
        protected Color origColor;

        protected CanvasGroup cg;
        protected Draggable draggable;
        protected Image bg;
        protected RectTransform rt;
        protected Selectable selectable;
        protected Divider myDivider;

        public Divider MyDivider => myDivider;

        public void PreDrag()
        {
            myDivider?.transform.parent.gameObject.SetActive(false);
            bg.raycastTarget = false;
            cg.blocksRaycasts = false;
            rt.sizeDelta = new Vector2(collapsedWidth, rt.sizeDelta.y);
        }

        public void PostDrag()
        {
            myDivider?.transform.parent.gameObject.SetActive(true);
            bg.raycastTarget = true;
            cg.blocksRaycasts = true;
        }

        protected void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            draggable = GetComponent<Draggable>();
            bg = GetComponent<Image>();
            rt = GetComponent<RectTransform>();
            selectable = GetComponent<Selectable>();
        }

        protected void Init(Divider myDivider, CodeList codeList)
        {
            this.myDivider = myDivider;
            this.codeList = codeList;

            selectable.Init(codeList.SelectionManager);

            draggable.Init(codeList.Dividers, codeList.TrashSlots);
            draggable.filterFunc = (Draggable.Slot slot) => slot == myDivider;
            draggable.onDragStart = (eventData) =>
            {
                // NOTE: Having block handle drag logic for selectable isn't ideal
                if (selectable.Selected) {
                    selectable.OnDragStart(eventData);
                    return false;
                } else {
                    PreDrag();
                    return true;
                }

            };
            draggable.onDragEnd = PostDrag;
        }
    }
}