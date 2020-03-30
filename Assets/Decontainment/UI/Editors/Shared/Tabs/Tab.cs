using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public class Tab : MonoBehaviour
    {
        /// The UI container that will be pushed to the front when the tab is clicked
        [SerializeField]
        private Transform container = null;
        [SerializeField]
        private TabManager tabManager = null;
        [SerializeField]
        private bool focusOnStart = false;

        public bool IsFocused => tabManager.focusTab == this;

        void Start()
        {
            if (focusOnStart) {
                Focus();
            }
        }

        public void Focus()
        {
            container.SetAsLastSibling();
            tabManager.focusTab = this;
        }
    }
}