using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor
{
    public class LabelBlock : Block
    {
        public void Init(Label label, Divider myDivider)
        {
            base.Init(myDivider);

            string labelText = label.name + " (" + label.val + ")";
            GetComponentInChildren<TextMeshProUGUI>().text = labelText;
        }
    }
}