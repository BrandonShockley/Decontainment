using Asm;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class SlotField : Draggable.Slot
    {
        [SerializeField]
        private GameObject tokenPrefab = null;

        private Argument arg;
        private GameObject tokenGO;
        Vector2 origSize;

        private Outline outline;
        private RectTransform rt;
        private TMP_InputField inputField;

        void Awake()
        {
            outline = GetComponent<Outline>();
            rt = GetComponent<RectTransform>();
            inputField = GetComponent<TMP_InputField>();

            origSize = rt.sizeDelta;
        }

        public void Init(Argument arg)
        {
            this.arg = arg;
            Globals.slotFields.Add(this);

            if (arg.type == Argument.Type.IMMEDIATE) {
                inputField.text = arg.val.ToString();
            } else {
                InsertArg(arg);
                inputField.text = "0";
            }
            inputField.onEndEdit.AddListener((string val) => arg.val = int.Parse(val));
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
                token.Init(arg);
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

        private void Resize()
        {
            if (tokenGO == null) {
                rt.sizeDelta = origSize;
            } else {
                RectTransform tokenRT = tokenGO.GetComponent<RectTransform>();
                rt.sizeDelta = tokenRT.sizeDelta;
            }
        }
    }
}