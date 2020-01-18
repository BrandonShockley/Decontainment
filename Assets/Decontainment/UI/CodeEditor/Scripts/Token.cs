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
        public SlotField slotField;

        [SerializeField]
        private ArgTokenColorMap argTokenColorMap = null;

        private Argument arg;

        private CanvasGroup cg;
        private Draggable draggable;
        private Image image;

        void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            draggable = GetComponent<Draggable>();
            image = GetComponent<Image>();
        }

        public void Init(Argument initArg)
        {
            arg = initArg;

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

            draggable.Init(Globals.slotFields, Globals.trashSlots);
            draggable.onDragStart = () =>
            {
                image.raycastTarget = false;
                cg.blocksRaycasts = false;
            };
            draggable.onDragEnd = () =>
            {
                image.raycastTarget = true;
                cg.blocksRaycasts = true;
            };
            draggable.onDragSuccess = (Draggable.Slot slot) =>
            {
                if (slotField != null) {
                    arg = slotField.ReleaseArg();
                }
                ((SlotField)slot).InsertArg(arg, gameObject);
            };
            draggable.onDragTrash = (Draggable.Slot slot) =>
            {
                slotField?.ReleaseArg();
                Destroy(gameObject);
            };
        }
    }
}