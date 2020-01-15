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

        protected void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            draggable = GetComponent<Draggable>();
            bg = GetComponent<Image>();
            rt = GetComponent<RectTransform>();
        }

        public void Init(List<RectTransform> dividerRTs, RectTransform myDivider, Action<RectTransform> onDragSuccess)
        {
            draggable.Init(dividerRTs);
            draggable.filterFunc = (RectTransform rt) => rt == myDivider;
            draggable.onDragStart = () =>
            {
                myDivider.gameObject.SetActive(false);
                bg.raycastTarget = false;
                cg.blocksRaycasts = false;
                rt.sizeDelta = new Vector2(collapsedWidth, rt.sizeDelta.y);
            };
            draggable.onDragCancel = () =>
            {
                myDivider.gameObject.SetActive(true);
                bg.raycastTarget = true;
                cg.blocksRaycasts = true;
            };
            draggable.onDragEnter = (RectTransform rt) =>
            {
                Image image = rt.GetComponent<Image>();
                origColor = image.color;
                image.color = dragOverColor;
            };
            draggable.onDragExit = (RectTransform rt) => rt.GetComponent<Image>().color = origColor;
            draggable.onDragSuccess = onDragSuccess;
        }
    }
}