using Asm;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor
{
    public class ConstLabelValueField : MonoBehaviour
    {
        private Label label;

        private RectTransform rt;
        private TextMeshProUGUI tm;
        private TMP_InputField inputField;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
            tm = GetComponentInChildren<TextMeshProUGUI>();
            inputField = GetComponent<TMP_InputField>();

        }

        void Start()
        {
            inputField.onEndEdit.AddListener(ChangeLabelValue);
            inputField.onValueChanged.AddListener(Resize);

            Token token = transform.parent.GetComponentInChildren<Token>();
            Debug.Assert(token.Arg.type == Argument.Type.LABEL);
            Debug.Assert(token.Arg.label.type == Label.Type.CONST);

            label = token.Arg.label;
            inputField.text = label.val.ToString();
        }

        private void Resize(string newText = null)
        {
            string text = newText == null ? tm.text : newText;
            RectTransform tmRT = tm.GetComponent<RectTransform>();
            float preferredWidth = tm.GetPreferredValues(text + "C").x; // Extra character to account for caret
            float widthDifference = rt.GetWorldSize().x - tmRT.GetWorldSize().x;
            float totalWidth = preferredWidth + widthDifference;

            rt.sizeDelta = new Vector2(totalWidth, rt.sizeDelta.y);
        }

        private void ChangeLabelValue(string newVal)
        {
            label.val = int.Parse(newVal);
        }
    }
}
