using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asm;
using TMPro;

namespace Editor
{
    public class ToolTipContainer : MonoBehaviour
    {
        public OpCode opCode;
        private TextMeshProUGUI textObject;

        void Awake()
        {
            textObject = GetComponentInChildren<TextMeshProUGUI>();
            string buffer;
            InstructionMaps.opDescriptiveNameMap.TryGetValue(opCode,out buffer);
            textObject.text = buffer;
        }

        public void setText(string text)
        {
            textObject.text = text;
        }
    }
}

