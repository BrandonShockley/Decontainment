using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Editor
{
    public class ListEntry : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Color selectedColor = Color.white;

        private Color deselectedColor;
        private Action onSelect;

        private Image image;
        private Renamable rn;
        private TMP_InputField inputField;

        void Awake()
        {
            image = GetComponent<Image>();
            rn = GetComponent<Renamable>();
            inputField = GetComponent<TMP_InputField>();

            deselectedColor = image.color;
        }

        public void Init(string name, Action<int> handleSelect, Func<int, string, bool> handleRename)
        {
            inputField.text = name;
            onSelect = () => handleSelect(transform.GetSiblingIndex());
            rn.onRename = (string newName) => handleRename(transform.GetSiblingIndex(), newName);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) {
                Select();
            }
        }

        public void Select()
        {
            image.color = selectedColor;
            onSelect?.Invoke();
        }

        public void Deselect()
        {
            image.color = deselectedColor;
        }
    }
}