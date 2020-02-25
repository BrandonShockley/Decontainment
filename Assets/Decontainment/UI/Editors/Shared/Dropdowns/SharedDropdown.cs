using TMPro;
using UnityEngine;

namespace Editor
{
    public class SharedDropdown : MonoBehaviour
    {
        protected EditorList

        protected TMP_Dropdown dropdown;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }


    }
}