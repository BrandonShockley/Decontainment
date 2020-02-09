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
        private CodeList codeList;

        private CanvasGroup cg;
        private Draggable draggable;
        private Image image;
        private RectTransform rt;
        private Renamable rn;
        private TextMeshProUGUI tm;
        private TMP_InputField inputField;

        public Argument Arg { get { return arg; } }

        void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            draggable = GetComponent<Draggable>();
            image = GetComponent<Image>();
            rt = GetComponent<RectTransform>();
            rn = GetComponent<Renamable>();
            tm = GetComponentInChildren<TextMeshProUGUI>();
            inputField = GetComponent<TMP_InputField>();

            rn.OnRename += RenameLabel;
        }

        public void Init(Argument initArg, CodeList codeList)
        {
            arg = initArg;
            this.codeList = codeList;

            // Configure text
            if (arg.type == Argument.Type.REGISTER) {
                inputField.text = "R" + arg.val.ToString();
            } else {
                inputField.text = arg.label.name;
            }

            // Configure token color
            ArgTokenColorMap.Type tokenType = arg.type == Argument.Type.REGISTER
                ? ArgTokenColorMap.Type.REGISTER
                : arg.label.type == Label.Type.BRANCH
                ? ArgTokenColorMap.Type.BRANCH_LABEL
                : ArgTokenColorMap.Type.CONST_LABEL;
            image.color = argTokenColorMap.map[tokenType];

            // Configure callbacks n' stuff
            draggable.Init(codeList.SlotFields, codeList.TrashSlots);
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

        private void RenameLabel(string newName)
        {
            if (newName == arg.label.name) {
                return;
            }

            // Make sure we're not renaming to a preexisting label or an invalid name
            if (codeList.Program.labelMap.ContainsKey(newName) || newName == "") {
                // TODO: Display a prompt when this happens (Trello #18)
                inputField.text = arg.label.name;
                return;
            }

            codeList.Program.labelMap.Remove(arg.label.name);
            arg.label.name = newName;
            codeList.Program.labelMap.Add(newName, arg.label);

            Action labelChangeAction;
            if (arg.label.type == Label.Type.BRANCH) {
                labelChangeAction = codeList.Program.BroadcastBranchLabelChange;
            } else {
                labelChangeAction = codeList.Program.BroadcastConstLabelChange;
            }

            labelChangeAction();
        }

    }
}