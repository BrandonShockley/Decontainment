using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public abstract class Block : MonoBehaviour
    {
        [SerializeField]
        protected float collapsedWidth = 44.0f;
        [SerializeField]
        protected Color dragOverColor = Color.white;

        protected Color origColor;

        protected CanvasGroup cg;
        protected Draggable draggable;
        protected Image bg;
        protected RectTransform rt;
        protected Divider myDivider;

        protected void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            draggable = GetComponent<Draggable>();
            bg = GetComponent<Image>();
            rt = GetComponent<RectTransform>();
        }

        public void Init(Divider myDivider)
        {
            this.myDivider = myDivider;

            draggable.Init(Globals.dividers, Globals.trashSlots);
            draggable.filterFunc = (Draggable.Slot slot) => slot == myDivider;
            draggable.onDragStart = HandleDragStart;
            draggable.onDragEnd = () =>
            {
                myDivider?.gameObject.SetActive(true);
                bg.raycastTarget = true;
                cg.blocksRaycasts = true;
            };
        }

        public void HandleDragStart()
        {
            myDivider?.gameObject.SetActive(false);
            bg.raycastTarget = false;
            cg.blocksRaycasts = false;
            rt.sizeDelta = new Vector2(collapsedWidth, rt.sizeDelta.y);
        }
    }
}