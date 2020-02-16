using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Editor
{
    public class Token : MonoBehaviour, IPointerClickHandler
    {
        public SlotField slotField;
        public bool renamable;

        [SerializeField]
        private ArgTokenColorMap argTokenColorMap = null;

        private Argument arg;

        private CanvasGroup cg;
        private Draggable draggable;
        private Image image;
        private RectTransform rt;
        private TextMeshProUGUI tm;
        private TMP_InputField inputField;

        public Argument Arg { get { return arg; }}

        void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            draggable = GetComponent<Draggable>();
            image = GetComponent<Image>();
            rt = GetComponent<RectTransform>();
            tm = GetComponentInChildren<TextMeshProUGUI>();
            inputField = GetComponent<TMP_InputField>();
        }

        public void Init(Argument initArg, bool renamable = false)
        {
            arg = initArg;
            this.renamable = renamable;

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

            // Resize to fit the preferred width
            Resize();

            // Configure callbacks n' stuff
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

            inputField.onDeselect.AddListener(RenameLabel);
            inputField.onSubmit.AddListener((string val) => EventSystem.current.SetSelectedGameObject(null));
            inputField.onValueChanged.AddListener((string newVal) => Resize());
            inputField.onValidateInput = ValidateInput;
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            bool isRightClick = pointerEventData.button == PointerEventData.InputButton.Right;
            if (renamable && (pointerEventData.clickCount == 2 || isRightClick)) {
                Debug.Assert(arg.type == Argument.Type.LABEL);

                inputField.interactable = true;
                inputField.ActivateInputField();
                inputField.interactable = false;
            }
        }

        private char ValidateInput(string text, int pos, char ch)
        {
            bool isNumber = ch >= '0' && ch <= '9';
            bool isUpperAlpha = ch >= 'A' && ch <= 'Z';
            bool isLowerAlpha = ch >= 'a' && ch <= 'z';
            bool isExtra = ch == '_' || ch == '-';

            if (isNumber || isUpperAlpha || isLowerAlpha || isExtra) {
                return ch;
            } else {
                return (char)0;
            }
        }

        private void Resize()
        {
            rt.sizeDelta = new Vector2(tm.GetPreferredValues(inputField.text).x, rt.sizeDelta.y);
        }

        private void RenameLabel(string newName)
        {
            if (newName == arg.label.name) {
                return;
            }
            // Make sure we're not renaming to a preexisting label or an invalid name
            if (codeList.Program.labelMap.ContainsKey(newName) || newName == "") {
                inputField.text = arg.label.name;
                PromptSystem.Instance.PromptInvalidLabelName(newName);
                return false;
            }

            Globals.program.labelMap.Remove(arg.label.name);
            arg.label.name = newName;
            Globals.program.labelMap.Add(newName, arg.label);

            Action labelChangeAction;
            if (arg.label.type == Label.Type.BRANCH) {
                labelChangeAction = Globals.program.BroadcastBranchLabelChange;
            } else {
                labelChangeAction = Globals.program.BroadcastConstLabelChange;
            }

            labelChangeAction();
        }

    }
}
