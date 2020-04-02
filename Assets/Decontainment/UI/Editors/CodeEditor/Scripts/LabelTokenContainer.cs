using Asm;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
{
    public class LabelTokenContainer : MonoBehaviour
    {
        [SerializeField]
        private Button removeButton = null;
        [SerializeField]
        private GameObject equalsText = null;
        [SerializeField]
        private TMP_InputField inputField = null;

        private Label label;
        private CodeList codeList;

        private RectTransform inputFieldRT;
        private TextMeshProUGUI inputFieldTM;

        void Awake()
        {
            inputFieldRT = inputField.GetComponent<RectTransform>();
            inputFieldTM = inputField.GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            inputField.onEndEdit.AddListener(ChangeLabelValue);
            inputField.onValueChanged.AddListener(Resize);

            Token token = GetComponentInChildren<Token>();
            Debug.Assert(token.Arg.type == Argument.Type.LABEL);

            label = token.Arg.label;

            if (label.type == Label.Type.CONST) {
                equalsText.SetActive(true);
                inputField.gameObject.SetActive(true);
                inputField.text = label.val.ToString();
            }

            removeButton.onClick.AddListener(() =>
            {
                codeList.Program.RemoveLabel(label);
                if (label.type == Label.Type.BRANCH) {
                    codeList.Program.BroadcastBranchLabelChange();
                } else {
                    codeList.Program.BroadcastConstLabelChange();
                }
            });
        }

        public void Init(CodeList codeList)
        {
            this.codeList = codeList;
        }

        private void Resize(string newText = null)
        {
            string text = newText == null ? inputFieldTM.text : newText;
            RectTransform tmRT = inputFieldTM.GetComponent<RectTransform>();
            float preferredWidth = inputFieldTM.GetPreferredValues(text + "C").x; // Extra character to account for caret
            float widthDifference = inputFieldRT.GetWorldSize().x - tmRT.GetWorldSize().x;
            float totalWidth = preferredWidth + widthDifference;

            inputFieldRT.sizeDelta = new Vector2(totalWidth, inputFieldRT.sizeDelta.y);
        }

        private void ChangeLabelValue(string newVal)
        {
            label.val = int.Parse(newVal);
            codeList.Program.BroadcastConstLabelChange();
        }
    }
}
