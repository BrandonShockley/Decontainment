using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor
{
    public class LabelBlock : Block
    {
        public void Init(Label label, List<RectTransform> dividerRTs, RectTransform myDivider, Action<RectTransform> onDragSuccess)
        {
            base.Init(dividerRTs, myDivider, onDragSuccess);

            string labelText = label.name + " (" + label.val + ")";
            GetComponentInChildren<TextMeshProUGUI>().text = labelText;
        }
    }
}