using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Renamable : MonoBehaviour, IPointerClickHandler
{
    public bool autoResize = false;
    public bool extraValidation = false;

    private Trigger shouldDeactivate;
    private string prevText;

    private RectTransform rt;
    private TextMeshProUGUI tm;
    private TMP_InputField inputField;

    // Returns false if text should be reverted
    public Func<string, bool> onRename;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        tm = GetComponentInChildren<TextMeshProUGUI>();
        inputField = GetComponent<TMP_InputField>();

        inputField.onEndEdit.AddListener((string s) =>
        {
            if (onRename != null && !onRename.Invoke(s)) {
                inputField.text = prevText;
            }

            shouldDeactivate.Value = true;
        });
        if (autoResize) {
            inputField.onValueChanged.AddListener((string newVal) => Resize());
        }
        if (extraValidation) {
            inputField.onValidateInput = ValidateInput;
        }
    }

    void Start()
    {
        if (autoResize) {
            Resize();
        }
    }

    void Update()
    {
        if (shouldDeactivate.Value) {
            inputField.interactable = false;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        bool isRightClick = pointerEventData.button == PointerEventData.InputButton.Right;
        if (isRightClick) {
            inputField.interactable = true;
            inputField.ActivateInputField();
            prevText = inputField.text;
        }
    }

    private void Resize()
    {
        float preferredWidth = tm.GetPreferredValues(inputField.text).x;
        float newWidth = Mathf.Max(preferredWidth, tm.GetPreferredValues("C").x);
        rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);
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
}
