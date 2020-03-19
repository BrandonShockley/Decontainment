using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asm;
using TMPro;

namespace Editor
{
    public class ToolTipContainer : MonoBehaviour
    {
        private TextMeshProUGUI textObject;
        private RectTransform rt;

        void Awake()
        {
            textObject = GetComponentInChildren<TextMeshProUGUI>();
            rt = GetComponent<RectTransform>();
        }

        public void setText(string text)
        {
            textObject.text = text;
            rt.sizeDelta = new Vector2(textObject.preferredWidth, 0);
        }
    }
}

