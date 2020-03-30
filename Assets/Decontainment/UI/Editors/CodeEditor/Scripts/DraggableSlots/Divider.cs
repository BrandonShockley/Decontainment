using Asm;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
{
    public class Divider : ColorChangeSlot
    {
        public int lineNumber;
        public Label label;

        public void Init(int lineNumber, SelectionManager selectionManager, Label label = null)
        {
            this.lineNumber = lineNumber;
            this.label = label;
            GetComponent<Selectable>().Init(selectionManager);
        }
    }
}