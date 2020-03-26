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
        List<DisplaysTips> children;
        private bool hovered;
        private string tipText;
        private float hoverTimer;

        void Start()
        {
            children = new List<DisplaysTips>(GetComponentsInChildren<DisplaysTips>());
            children.RemoveAt(0); // Remove self
            canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        }

        void Update()
        {
            if (GlobalSettings.TooltipsEnabled && hovered && NoChildrenHovered()) {
                hoverTimer += Time.unscaledDeltaTime;
                if (toolTipInstance == null && hoverTimer > hoverDelay) {
                    InstantiateTip();
                }
            } else {
                hoverTimer = 0;
                if (toolTipInstance != null) {
                    Destroy(toolTipInstance);
                }
            }
        }

        public void Init(string text)
        {
            tipText = text;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
        }

        private bool NoChildrenHovered()
        {
            return children.TrueForAll((child) => !child.hovered);
        }

        private void InstantiateTip()
        {
            toolTipInstance = Instantiate(toolTipContainerPrefab, Input.mousePosition + Vector3.right * 10, Quaternion.identity, canvas.transform);
            toolTipInstance.GetComponent<ToolTipContainer>().SetText(tipText);
        }
    }
}
