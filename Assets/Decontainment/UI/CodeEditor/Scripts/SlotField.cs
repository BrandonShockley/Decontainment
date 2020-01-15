using Asm;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class SlotField : MonoBehaviour
    {
        [SerializeField]
        private GameObject tokenPrefab = null;

        private Argument arg;
        private GameObject tokenGO;
        private List<RectTransform> slotRTs;
        Vector2 origSize;

        private RectTransform rt;
        private TMP_InputField inputField;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
            inputField = GetComponent<TMP_InputField>();

            origSize = rt.sizeDelta;
        }

        public void Init(Argument arg, List<RectTransform> slotRTs)
        {
            this.arg = arg;
            this.slotRTs = slotRTs;
            slotRTs.Add(GetComponent<RectTransform>());

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
                token.Init(arg, slotRTs);
            } else {
                // Transfer the one we're given
                tokenGO = transferedToken;
                tokenGO.transform.SetParent(transform, false);
                token = tokenGO.GetComponent<Token>();
            }

            Resize();
            inputField.interactable = false;
            token.slot = this;
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