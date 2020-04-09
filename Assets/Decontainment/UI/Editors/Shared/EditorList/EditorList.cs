using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public abstract class EditorList<T> : ReadOnlyEditorList<T> where T : class
    {
        public event Action<int> OnItemAdded;
        public event Action<int, T> OnItemDeleted;
        public event Action<string, int, int> OnItemRenamed;

        protected abstract string DefaultName { get; }

        public void Add()
        {
            string newName;
            for (int i = 0;; ++i) {
                newName = DefaultName + i.ToString();
                bool nameGood = true;
                foreach (T item in items) {
                    if (item.ToString() == newName) {
                        nameGood = false;
                        break;
                    }
                }

                if (nameGood) {
                    break;
                }
            }

            T newItem = CreateNewItem(newName);
            int index = items.InsertAlphabetically(newItem);

            if (index <= selectedIndex) {
                ++selectedIndex;
            }

            CreateListEntry(newItem, index);
            SubAdd(index);
            OnItemAdded?.Invoke(index);
        }

        public void Delete()
        {
            if (selectedIndex == -1) {
                return;
            }

            int removalIndex = selectedIndex;
            selectedIndex = -1;
            Destroy(transform.GetChild(removalIndex).gameObject);
            T item = items[removalIndex];
            items.RemoveAt(removalIndex);
            DeleteItem(item);

            SubDelete(removalIndex, item);
            OnItemDeleted?.Invoke(removalIndex, item);
            HandleSelect(-1);
        }

        protected bool HandleRename(int index, string name)
        {
            // Check that we don't override an existing item
            foreach (T item in items) {
                if (item.ToString() == name) {
                    return false;
                }
            }

            {
                T item = items[index];
                string oldName = item.ToString();
                items.RemoveAt(index);
                RenameItem(item, name);
                int newIndex = items.InsertAlphabetically(item);

                transform.GetChild(index).SetSiblingIndex(newIndex);

                if (index == selectedIndex) {
                    selectedIndex = newIndex;
                } else if (index > selectedIndex && newIndex <= selectedIndex) {
                    ++selectedIndex;
                } else if (index < selectedIndex && newIndex > selectedIndex) {
                    --selectedIndex;
                }

                SubHandleRename(oldName, index, newIndex);
                OnItemRenamed?.Invoke(oldName, index, newIndex);
            }
            return true;
        }

        protected virtual void SubAdd(int newIndex) {}
        protected virtual void SubDelete(int oldIndex, T oldItem) {}
        protected virtual void SubHandleRename(string oldName, int oldIndex, int newIndex) {}

        protected override void CreateListEntry(T item, int siblingIndex = -1)
        {
            GameObject listEntryGO = Instantiate(listEntryPrefab, transform);
            TextListEntry listEntry = listEntryGO.GetComponent<TextListEntry>();
            listEntry.Init(item.ToString(), HandleRename);
            listEntry.OnSelect += () => HandleSelect(listEntryGO.transform.GetSiblingIndex());
            if (siblingIndex >= 0) {
                listEntry.transform.SetSiblingIndex(siblingIndex);
            }
        }

        protected abstract T CreateNewItem(string name);
        protected abstract void DeleteItem(T item);
        protected abstract void RenameItem(T item, string name);
    }
}