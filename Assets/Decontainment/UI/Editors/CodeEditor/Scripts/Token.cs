using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
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

            rn.onRename = RenameLabel;
        }

        public void Init(Argument initArg, CodeList codeList, SlotField slotField)
        {
            arg = initArg;
            this.codeList = codeList;
            this.slotField = slotField;

            // Configure text
            if (arg.type == Argument.Type.REGISTER) {
                inputField.text = "R" + arg.val.ToString();
            } else {
                inputField.text = arg.label.name;
            }

            // Configure token color
            ArgTokenColorMap.Type tokenType = arg.type == Argument.Type.REGISTER
                ? arg.val < VirtualMachine.NUM_LOCAL_REGS
                ? ArgTokenColorMap.Type.LOCAL_REGISTER
                : ArgTokenColorMap.Type.SHARED_REGISTER
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
                ((SlotField)slot).InsertArg(arg);
                if (slotField == null) {
                    Destroy(gameObject);
                }
            };
            draggable.onDragTrash = (Draggable.Slot slot) =>
            {
                slotField?.ReleaseArg();
                Destroy(gameObject);
            };
        }

        private bool RenameLabel(string newName)
        {
            if (newName == arg.label.name) {
                return true;
            }

            // Make sure we're not renaming to a preexisting label or an invalid name
            if (codeList.Program.labelMap.ContainsKey(newName) || newName == "") {
                inputField.text = arg.label.name;
                PromptSystem.Instance.PromptInvalidLabelName(newName);
                return false;
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
            return true;
        }

    }
}