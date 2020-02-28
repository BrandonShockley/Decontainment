using Asm;
using Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Editor.Code
{
    public class SlotField : Draggable.Slot
    {
        [SerializeField]
        private GameObject tokenPrefab = null;

        private Argument arg;
        private CodeList codeList;
        private GameObject tokenGO;
        private Vector2 origSize;

        private Outline outline;
        private RectTransform rt;
        private TMP_InputField inputField;
        private TextMeshProUGUI tm;

        void Awake()
        {
            outline = GetComponent<Outline>();
            rt = GetComponent<RectTransform>();
            inputField = GetComponent<TMP_InputField>();
            tm = GetComponentInChildren<TextMeshProUGUI>();

            origSize = rt.sizeDelta;
        }

        void OnDestroy()
        {
            if (arg != null) {
                arg.OnChange -= UpdateFrontend;
            }
        }

        public void Init(Argument arg, CodeList codeList)
        {
            this.arg = arg;
            this.codeList = codeList;
            codeList.SlotFields.Add(this);

            inputField.onEndEdit.AddListener((string val) =>
            {
                arg.val = int.Parse(val);
                arg.BroadcastChange();
                codeList.Program.BroadcastArgumentChange();
            });

            arg.OnChange += UpdateFrontend;
            UpdateFrontend();
            inputField.onValueChanged.AddListener(Resize);
        }

        public void InsertArg(Argument newArg)
        {
            Debug.Assert(newArg.type != Argument.Type.IMMEDIATE);

            arg.CopyValues(newArg);
            arg.BroadcastChange();
            codeList.Program.BroadcastArgumentChange();
        }

        public Argument ReleaseArg()
        {
            Argument releasedArg = arg.ShallowCopy();

            arg.type = Argument.Type.IMMEDIATE;
            arg.val = int.Parse(inputField.text);
            arg.BroadcastChange();
            codeList.Program.BroadcastArgumentChange();

            return releasedArg;
        }

        public override void HandleDragEnter()
        {
            outline.enabled = true;
        }

        public override void HandleDragExit()
        {
            outline.enabled = false;
        }

        private void Resize(string newText = null)
        {
            if (tokenGO == null) {
                string text = newText == null ? tm.text : newText;
                RectTransform tmRT = tm.GetComponent<RectTransform>();
                float preferredWidth = tm.GetPreferredValues(text + "C").x; // Extra character to account for caret
                float widthDifference = rt.GetWorldSize().x - tmRT.GetWorldSize().x;

                rt.sizeDelta = new Vector2(preferredWidth + widthDifference, rt.sizeDelta.y);
            } else {
                RectTransform tokenRT = tokenGO.GetComponent<RectTransform>();
                rt.sizeDelta = tokenRT.sizeDelta;
            }
        }

        private void UpdateFrontend()
        {
            if (arg.type == Argument.Type.IMMEDIATE) {
                if (tokenGO != null) {
                    Destroy(tokenGO);
                    tokenGO = null;
                }
                inputField.text = arg.val.ToString();
                inputField.interactable = true;
            } else {
                if (tokenGO == null) {
                    tokenGO = Instantiate(tokenPrefab, transform, false);
                }
                tokenGO.GetComponent<Token>().Init(arg, codeList, this);
                inputField.text = "0";
                inputField.interactable = false;
            }
            Resize();
        }
    }
}