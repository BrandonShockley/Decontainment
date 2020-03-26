using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor
{
    public class NoItemIndicator<T, TL> : MonoBehaviour
    where T : class
    where TL : EditorList<T>
    {
        [SerializeField]
        private TL editorList = null;

        private TextMeshProUGUI tm;

        void Awake()
        {
            tm = GetComponent<TextMeshProUGUI>();

            editorList.OnItemSelected += HandleSelection;
        }

        void Start()
        {
            HandleSelection(-1);
        }

        private void HandleSelection(int oldIndex)
        {
            tm.enabled = editorList.SelectedItem == null;
        }
    }
}
