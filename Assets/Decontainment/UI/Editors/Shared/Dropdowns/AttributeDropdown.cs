using Extensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor
{
    public abstract class AttributeDropdown<T, TL, A, AL> : MonoBehaviour
        where T : class // Target
        where TL : EditorList<T> // Target editor list
        where A : class // Attribute
        where AL : IReadOnlyList<A> // Attribute collection
    {
        private const string NULL_STRING = "[None]";

        /// Editor list handling things affected by the dropdown
        [SerializeField]
        protected TL targetEditorList = default;

        /// Collection handling things displayed by the dropdown
        [SerializeField]
        protected AL attributes = default;

        protected Trigger selfChange;
        protected T currentTarget;

        protected TMP_Dropdown dropdown;

        protected abstract string AttributeName { get; }

        protected void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();

            targetEditorList.OnItemSelected += (int oldIndex) => HandleTargetSelected();
            targetEditorList.OnItemDeleted += (int oldIndex, T oldItem) => HandleTargetSelected();

            dropdown.onValueChanged.AddListener((int val) =>
            {
                if (selfChange.Value) {
                    return;
                }

                selfChange.Value = true;
                if (val == dropdown.options.Count - 1) {
                    ClearAttribute();
                } else {
                    SetAttribute(val);
                }
            });

            SubAwake();
        }

        protected void Start()
        {
            for (int i = 0; i < attributes.Count; ++i) {
                HandleAttributeItemAdded(i);
            }
            dropdown.options.Add(new TMP_Dropdown.OptionData(NULL_STRING));

            HandleTargetSelected();
        }

        protected void OnDestroy()
        {
            if (currentTarget != null) {
                UnregisterChangeHandler();
            }
        }

        protected void HandleAttributeItemAdded(int index)
        {
            dropdown.options.Insert(index, new TMP_Dropdown.OptionData(attributes[index].ToString()));
            if (index <= dropdown.value) {
                selfChange.Value = true;
                ++dropdown.value;
            }
        }

        protected void HandleAttributeItemDeleted(int index, A item)
        {
            dropdown.options.RemoveAt(index);
            if (index < dropdown.value) {
                selfChange.Value = true;
                --dropdown.value;
            }
        }

        protected void HandleAttributeItemRenamed(string oldName, int oldIndex, int newIndex)
        {
            dropdown.options.RemoveAt(oldIndex);
            HandleAttributeItemAdded(newIndex);
        }

        protected void HandleAttributeChanged()
        {
            if (selfChange.Value) {
                return;
            }

            int newValue;
            if (targetEditorList.SelectedItem == null) {
                newValue = dropdown.options.Count - 1;
            } else {
                int index = attributes.FindIndex(
                    (A attribute) => attribute.ToString() == AttributeName);

                if (index == -1) {
                    index = dropdown.options.Count - 1;
                }

                newValue = index;
            }

            if (newValue != dropdown.value) {
                selfChange.Value = true;
                dropdown.value = newValue;
            }
        }

        protected virtual void SubAwake() {}
        protected virtual void SubHandleTargetSelected() {}

        /// Should clear the currently selected item's attribute
        protected abstract void ClearAttribute();
        /// Should set the currently selected item's attribute
        protected abstract void SetAttribute(int index);
        protected abstract void RegisterChangeHandler();
        protected abstract void UnregisterChangeHandler();

        private void HandleTargetSelected()
        {
            if (currentTarget != null) {
                UnregisterChangeHandler();
            }

            currentTarget = targetEditorList.SelectedItem;

            if (currentTarget == null) {
                dropdown.interactable = false;
            } else {
                dropdown.interactable = true;
                RegisterChangeHandler();
            }

            HandleAttributeChanged();
            SubHandleTargetSelected();
        }


    }
}