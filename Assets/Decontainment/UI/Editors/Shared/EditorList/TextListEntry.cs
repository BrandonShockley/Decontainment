using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Editor
{
    public class TextListEntry : MonoBehaviour, IListEntry, IPointerClickHandler
    {
        [SerializeField]
        private float selectedColorMultiplier = 1;

        private Color deselectedColor;
        private bool selected;

        private Image image;
        private Renamable rn;
        private TMP_InputField inputField;

        public event Action OnSelect;

        void Awake()
        {
            image = GetComponent<Image>();
            rn = GetComponent<Renamable>();
            inputField = GetComponent<TMP_InputField>();
        }

        void Start()
        {
            deselectedColor = image.color;
        }

        public void Init(string name, Func<int, string, bool> handleRename)
        {
            inputField.text = name;
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
            if (!selected) {
                selected = true;
                image.color *= selectedColorMultiplier;
                OnSelect?.Invoke();
            }
        }

        public void Deselect()
        {
            if (selected) {
                selected = false;
                image.color = deselectedColor;
            }
        }
    }
}