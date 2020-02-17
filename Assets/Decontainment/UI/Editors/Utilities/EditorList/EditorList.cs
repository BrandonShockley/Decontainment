using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Editor
{
    public abstract class EditorList<T> : MonoBehaviour
    {
        protected List<T> items = new List<T>();

        [SerializeField]
        private GameObject listEntryPrefab = null;

        private int _selectedIndex = -1;

        protected int SelectedIndex
        {
            get { return _selectedIndex; }
            set {
                if (_selectedIndex != -1) {
                    transform.GetChild(_selectedIndex).GetComponent<ListEntry>().Deselect();
                }

                _selectedIndex = value;
            }
        }
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
        }

        public void Remove()
        {
            if (SelectedIndex == -1) {
                return;
            }

            int removalIndex = SelectedIndex;
            SelectedIndex = -1;
            Destroy(transform.GetChild(removalIndex).gameObject);
            T item = items[removalIndex];
            items.RemoveAt(removalIndex);
            DeleteItem(item);
        }

        protected void Awake()
        {
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
                items.RemoveAt(index);
                RenameItem(item, name);
                int newIndex = items.InsertAlphabetically(item);

                transform.GetChild(index).SetSiblingIndex(newIndex);

                if (index == SelectedIndex) {
                    _selectedIndex = newIndex;
                }
            }
            return true;
        }

        protected void HandleSelect(int index)
        {
            SelectedIndex = index;
            SubHandleSelect();
        }

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