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
        [SerializeField]
        private GameObject toolTipContainerPrefab = null;
        [SerializeField]
        private float hoverDelay = 0;

        private GameObject toolTipInstance;
        private GameObject canvas;
        DisplaysTips parent;
        private bool hovered;
        private bool isField;
        private string tipText;
        private float hoverTimer;

        void Awake()
        {
            isField = (GetComponent<SlotField>() != null || GetComponent<DropdownField>() != null);
            if (isField) {
                parent = transform.parent.GetComponentInParent<DisplaysTips>();
            }
            canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        }

        public void Init(string text)
        {
            tipText = text;
        }

        void Update()
        {
            if (hovered && GlobalSettings.TooltipsEnabled) {
                hoverTimer += Time.unscaledDeltaTime;
                if (toolTipInstance == null && hoverTimer > hoverDelay) {
                    InstantiateTip();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            hoverTimer = 0;

            if (isField) {
                // If entering a slot, destroy the instruction block tip
                parent.OnPointerExit(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;

            if (toolTipInstance != null) {
                Destroy(toolTipInstance);
            }

            if (isField) {
                // If exiting a slot, instruction block is now hovered over
                parent.OnPointerEnter(eventData);
            }
        }

        public void InstantiateTip()
        {
            toolTipInstance = Instantiate(toolTipContainerPrefab, Input.mousePosition + Vector3.right * 10, Quaternion.identity, canvas.transform);
            toolTipInstance.GetComponent<ToolTipContainer>().SetText(tipText);
        }
    }
}
