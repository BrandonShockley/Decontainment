using System;
using TMPro;
using UnityEngine;

namespace Editor
{
    /// Dynamic dropdowns can have their options changed
    /// by other editor lists
    public abstract class DynamicAttributeDropdown<T, TL, A, AL> : MonoBehaviour
    where T : class // Target
    where TL : EditorList<T> // Target editor list
    where A : class // Attribute
    where AL : EditorList<A> // Attribute editor list
    {
        private const string NULL_STRING = "[None]";

        /// Editor list handling things affected by the dropdown
        [SerializeField]
        protected TL targetEditorList = null;

        /// Editor list handling things displayed by the dropdown
        [SerializeField]
        protected AL attributeEditorList = null;

        protected Trigger selfChange;

        protected TMP_Dropdown dropdown;

        protected abstract string AttributeName { get; }

        protected void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();

            targetEditorList.OnItemSelected += HandleTargetItemSelected;

            attributeEditorList.OnItemAdded += HandleAttributeItemAdded;
            attributeEditorList.OnItemDeleted += HandleAttributeItemDeleted;
            attributeEditorList.OnItemRenamed += HandleAttributeItemRenamed;

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
        }

        protected void Start()
        {
            for (int i = 0; i < attributeEditorList.Count; ++i) {
                HandleAttributeItemAdded(i);
            }
            dropdown.options.Add(new TMP_Dropdown.OptionData(NULL_STRING));

            HandleTargetItemSelected(-1);
        }

        protected abstract void ClearAttribute();
        protected abstract void SetAttribute(int index);
        protected abstract void RegisterChangeHandler(T sourceItem, Action changeHandler);
        protected abstract void UnregisterChangeHandler(T sourceItem, Action changeHandler);

        private void HandleTargetItemSelected(int oldIndex)
        {
            T oldItem = targetEditorList[oldIndex];
            if (oldItem != null) {
                UnregisterChangeHandler(oldItem, HandleAttributeItemChanged);
            }

            if (targetEditorList.SelectedItem == null) {
                dropdown.interactable = false;
            } else {
                dropdown.interactable = true;
                RegisterChangeHandler(targetEditorList.SelectedItem, HandleAttributeItemChanged);
            }

            HandleAttributeItemChanged();
        }

        private void HandleAttributeItemChanged()
        {
            if (selfChange.Value) {
                return;
            }

            int newValue;
            if (targetEditorList.SelectedItem == null) {
                newValue = dropdown.options.Count - 1;
            } else {
                int index = attributeEditorList.FindIndex(AttributeName);

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

        private void HandleAttributeItemAdded(int index)
        {
            dropdown.options.Insert(index, new TMP_Dropdown.OptionData(attributeEditorList.Index(index).ToString()));
            if (index <= dropdown.value) {
                selfChange.Value = true;
                ++dropdown.value;
            }
        }

        private void HandleAttributeItemDeleted(int index, A item)
        {
            dropdown.options.RemoveAt(index);
            if (index < dropdown.value) {
                selfChange.Value = true;
                --dropdown.value;
            }
        }

        private void HandleAttributeItemRenamed(string oldName, int oldIndex, int newIndex)
        {
            dropdown.options.RemoveAt(oldIndex);
            HandleAttributeItemAdded(newIndex);
        }
    }
}