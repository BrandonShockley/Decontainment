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

        public void BringToTop()
        {
            container.SetAsLastSibling();
        }
    }
}