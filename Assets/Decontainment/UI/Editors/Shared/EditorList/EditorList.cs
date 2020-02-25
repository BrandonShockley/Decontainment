using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public abstract class EditorList<T> : MonoBehaviour where T : class
    {
        protected List<T> items = new List<T>();

        [SerializeField]
        private GameObject listEntryPrefab = null;

        private int selectedIndex = -1;

        public event Action<int> OnItemAdded;
        public event Action<int, T> OnItemDeleted;
        public event Action<string, int, int> OnItemRenamed;
        public event Action OnItemSelected;

        public T SelectedItem { get { return selectedIndex == -1 ? null : items[selectedIndex]; } }
        public int Count { get { return items.Count; } }

        protected int SelectedIndex { get { return selectedIndex; } }
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
            CreateListEntry(newItem, index);
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
            OnItemDeleted?.Invoke(removalIndex, item);
        }

        public T Index(int index) { return items[index]; }

        public T Find(string name)
        {
            return items.Find((T t) => t.ToString() == name);
        }

        protected void Awake()
        {
            SubAwake();
            InitList();

            foreach (T item in items) {
                CreateListEntry(item);
            }
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
                }

                OnItemRenamed?.Invoke(oldName, index, newIndex);
            }
            return true;
        }

        protected void HandleSelect(int index)
        {
            if (selectedIndex != -1 && selectedIndex != index) {
                transform.GetChild(selectedIndex).GetComponent<ListEntry>().Deselect();
            }
            selectedIndex = index;
            SubHandleSelect();
            OnItemSelected?.Invoke();
        }

        protected virtual void SubAwake() {}

        protected abstract void InitList();
        protected abstract T CreateNewItem(string name);
        protected abstract void DeleteItem(T item);
        protected abstract void RenameItem(T item, string name);
        protected abstract void SubHandleSelect();

        private void CreateListEntry(T item, int siblingIndex = -1)
        {
            GameObject listEntry = Instantiate(listEntryPrefab, transform);
            listEntry.GetComponent<ListEntry>().Init(item.ToString(), HandleSelect, HandleRename);
            if (siblingIndex >= 0) {
                listEntry.transform.SetSiblingIndex(siblingIndex);
            }
        }
    }
}