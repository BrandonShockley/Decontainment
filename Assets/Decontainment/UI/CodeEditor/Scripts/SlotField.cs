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
        private ArgTokenColorMap argTokenColorMap = null;
        [SerializeField]
        private GameObject tokenPrefab = null;

        private Argument arg;
        private GameObject token;
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

            if (token != null) {
                Destroy(token);
            }

            Draggable tokenDraggable;
            if (transferedToken == null) {
                // Create a new one
                token = Instantiate(tokenPrefab, transform, false);
                tokenDraggable = token.GetComponent<Draggable>();
                tokenDraggable.Init(slotRTs);

                // Configure token text
                TextMeshProUGUI tm = token.GetComponentInChildren<TextMeshProUGUI>();
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
                token.GetComponent<Image>().color = argTokenColorMap.map[tokenType];

                // Resize to fit the preferred width
                RectTransform tokenRT = token.GetComponent<RectTransform>();
                tokenRT.sizeDelta = new Vector2(tm.GetPreferredValues(tm.text).x, rt.sizeDelta.y);
            } else {
                // Transfer the one we're given
                token = transferedToken;
                token.transform.SetParent(transform, false);
                tokenDraggable = token.GetComponent<Draggable>();
            }

            Resize();
            inputField.interactable = false;

            tokenDraggable.onDragEnter = (RectTransform slotRT) => slotRT.GetComponent<Outline>().enabled = true;
            tokenDraggable.onDragExit = (RectTransform slotRT) => slotRT.GetComponent<Outline>().enabled = false;
            tokenDraggable.onDragSuccess = (RectTransform slotRT) =>
            {
                GameObject oldToken = token; // ReleaseArg will set token to null
                slotRT.GetComponent<SlotField>().InsertArg(ReleaseArg(), oldToken);
            };
        }

        public Argument ReleaseArg()
        {
            Debug.Assert(token != null);

            Argument releasedArg = arg.ShallowCopy();

            // Reconfigure for text field mode
            token = null;
            arg.type = Argument.Type.IMMEDIATE;
            arg.val = int.Parse(inputField.text);
            inputField.interactable = true;
            Resize();

            return releasedArg;
        }

        private void Resize()
        {
            if (token == null) {
                rt.sizeDelta = origSize;
            } else {
                RectTransform tokenRT = token.GetComponent<RectTransform>();
                rt.sizeDelta = tokenRT.sizeDelta;
            }
        }
    }
}