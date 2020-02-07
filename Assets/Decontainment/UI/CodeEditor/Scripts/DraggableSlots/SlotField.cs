using Asm;
using Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Editor
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

        public void Init(Argument arg, CodeList codeList)
        {
            this.arg = arg;
            this.codeList = codeList;
            codeList.SlotFields.Add(this);

            inputField.onEndEdit.AddListener((string val) =>
            {
                arg.val = int.Parse(val);
            });

            if (arg.type == Argument.Type.IMMEDIATE) {
                inputField.text = arg.val.ToString();
                Resize();
            } else {
                InsertArg(arg);
                inputField.text = "0";
            }
            inputField.onValueChanged.AddListener(Resize);
        }

        public void InsertArg(Argument newArg, GameObject transferedToken = null)
        {
            Debug.Assert(newArg.type != Argument.Type.IMMEDIATE);

            arg.CopyValues(newArg);

            if (tokenGO != null) {
                Destroy(tokenGO);
            }

            Token token;
            if (transferedToken == null) {
                // Create a new one
                tokenGO = Instantiate(tokenPrefab, transform, false);
                token = tokenGO.GetComponent<Token>();
                token.Init(arg, codeList);
            } else {
                // Transfer the one we're given
                tokenGO = transferedToken;
                token = tokenGO.GetComponent<Token>();

                // Center it
                RectTransform tokenRT = tokenGO.GetComponent<RectTransform>();
                tokenRT.SetParent(transform, false);
                tokenRT.anchorMin = new Vector2(0.5f, 0.5f);
                tokenRT.anchorMax = new Vector2(0.5f, 0.5f);
                tokenRT.anchoredPosition = Vector2.zero;
            }

            Resize();
            inputField.interactable = false;
            token.slotField = this;
        }

        public Argument ReleaseArg()
        {
            Debug.Assert(tokenGO != null);

            Argument releasedArg = arg.ShallowCopy();

            // Reconfigure for text field mode
            tokenGO = null;
            arg.type = Argument.Type.IMMEDIATE;
            arg.val = int.Parse(inputField.text);
            inputField.interactable = true;
            Resize();

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
    }
}