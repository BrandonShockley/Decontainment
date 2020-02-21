using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Editor.Code;
using Asm;

namespace Editor
{
    public class DisplaysTips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public GameObject toolTipContainer;

        private GameObject toolTipInstance;
        private GameObject canvas;
        private int t;
        private bool hovered;
        private bool isSlot;
        private string tipText;

        private const int tipDelay = 40;

        void Awake()
        {
            canvas = GameObject.Find("Canvas");
            isSlot = (GetComponent<SlotField>() != null) ? true : false;
        }

        public void Init(string text)
        {
            tipText = text;
        }

        void Update()
        {
            if (hovered)
            {
                if ((toolTipInstance == null) && (t > tipDelay))
                {
                    if (tipText != null)
                    {
                        InstantiateTip();
                    }
                } else
                {

                    t++;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            t = 0;

            if (isSlot)
            {
                //if entering a slot, destroy the instruction block tip
                DisplaysTips[] parents = GetComponentsInParent<DisplaysTips>();
                DisplaysTips parent = parents[parents.Length - 1];
                GameObject destroyedInstance = parent.toolTipInstance;
                Destroy(destroyedInstance);
                parent.hovered = false;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            t = 0;

            if (toolTipInstance != null)
            {
                Destroy(toolTipInstance);
            }

            if (isSlot)
            {
                //if exiting a slot, instruction block is now hovered over
                DisplaysTips[] parents = GetComponentsInParent<DisplaysTips>();
                DisplaysTips parent = parents[parents.Length - 1];
                parent.hovered = true;
            }
        }

        public void InstantiateTip()
        {
            toolTipInstance = Instantiate(toolTipContainer, Input.mousePosition + Vector3.right * 10, Quaternion.identity, canvas.transform);
            toolTipInstance.GetComponent<ToolTipContainer>().setText(tipText);
        }
    }
}
