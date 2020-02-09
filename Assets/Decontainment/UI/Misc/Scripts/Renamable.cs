using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Renamable : MonoBehaviour, IPointerClickHandler
{
    public bool autoResize = false;
    public bool extraValidation = false;

    private RectTransform rt;
    private TextMeshProUGUI tm;
    private TMP_InputField inputField;

    public event Action<string> OnRename;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        tm = GetComponentInChildren<TextMeshProUGUI>();
        inputField = GetComponent<TMP_InputField>();

        inputField.onDeselect.AddListener((string s) => OnRename?.Invoke(s));
        inputField.onSubmit.AddListener((string val) => EventSystem.current.SetSelectedGameObject(null));
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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        bool isRightClick = pointerEventData.button == PointerEventData.InputButton.Right;
        if (isRightClick) {
            inputField.interactable = true;
            inputField.ActivateInputField();
            inputField.interactable = false;
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
