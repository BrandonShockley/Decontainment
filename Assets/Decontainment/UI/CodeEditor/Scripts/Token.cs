using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class Token : MonoBehaviour
    {
        public SlotField slot;

        [SerializeField]
        private ArgTokenColorMap argTokenColorMap = null;

        private CanvasGroup cg;
        private Draggable draggable;
        private Image image;

        void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            draggable = GetComponent<Draggable>();
            image = GetComponent<Image>();
        }

        void Start()
        {
            draggable.onDragStart = () =>
            {
                image.raycastTarget = false;
                cg.blocksRaycasts = false;
            };
            draggable.onDragCancel = () =>
            {
                image.raycastTarget = true;
                cg.blocksRaycasts = true;
            };
            draggable.onDragEnter = (RectTransform slotRT) => slotRT.GetComponent<Outline>().enabled = true;
            draggable.onDragExit = (RectTransform slotRT) => slotRT.GetComponent<Outline>().enabled = false;
        }

        public void Init(Argument arg, List<RectTransform> slotRTs)
        {
            // Configure text
            TextMeshProUGUI tm = GetComponentInChildren<TextMeshProUGUI>();
            if (arg.type == Argument.Type.REGISTER) {
                tm.text = "R" + arg.val.ToString();
            } else {
                tm.text = arg.label.name;
            }

            // Configure token color
            ArgTokenColorMap.Type tokenType = arg.type == Argument.Type.REGISTER
                ? ArgTokenColorMap.Type.REGISTER
                : arg.label.type == Label.Type.BRANCH
                ? ArgTokenColorMap.Type.BRANCH_LABEL
                : ArgTokenColorMap.Type.CONST_LABEL;
            image.color = argTokenColorMap.map[tokenType];

            // Resize to fit the preferred width
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(tm.GetPreferredValues(tm.text).x, rt.sizeDelta.y);

            draggable.Init(slotRTs);
            draggable.onDragSuccess = (RectTransform slotRT) =>
            {
                GameObject oldToken = gameObject; // ReleaseArg will set token to null
                slotRT.GetComponent<SlotField>().InsertArg(slot.ReleaseArg(), oldToken);
            };
        }
    }
}