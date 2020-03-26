using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public abstract class ReadOnlyEditorList<T> : MonoBehaviour, IReadOnlyList<T> where T : class
    {
        protected List<T> items = new List<T>();

        [SerializeField]
        protected GameObject listEntryPrefab = null;

        protected int selectedIndex = -1;

        public event Action<int> OnItemSelected;

        public T this[int i] { get { return i == -1 ? null : items[i]; } }
        public T SelectedItem { get { return selectedIndex == -1 ? null : items[selectedIndex]; } }
        public int Count { get { return items.Count; } }

        protected int SelectedIndex { get { return selectedIndex; } }

        public T Find(string name)
        {
            return items.Find((T t) => t.ToString() == name);
        }

        public int FindIndex(string name)
        {
            return items.FindIndex((T t) => t.ToString() == name);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void Awake()
        {
            SubAwake();
            InitList();

            foreach (T item in items) {
                CreateListEntry(item);
            }
        }

        protected void HandleSelect(int index)
        {
            if (selectedIndex != -1 && selectedIndex != index) {
                transform.GetChild(selectedIndex).GetComponent<IListEntry>().Deselect();
            }
            int oldIndex = index;
            selectedIndex = index;
            SubHandleSelect(oldIndex);
            OnItemSelected?.Invoke(oldIndex);
        }

        protected virtual void SubAwake() {}
        protected virtual void SubHandleSelect(int oldIndex) {}

        protected abstract void CreateListEntry(T item, int siblingIndex = -1);

        protected abstract void InitList();
    }
}