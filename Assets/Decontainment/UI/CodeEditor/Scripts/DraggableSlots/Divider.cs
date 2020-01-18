using Asm;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class Divider : ColorChangeSlot
    {
        public int lineNumber;
        public Label label;

        public void Init(int lineNumber, Label label = null)
        {
            this.lineNumber = lineNumber;
            this.label = label;
        }
    }
}