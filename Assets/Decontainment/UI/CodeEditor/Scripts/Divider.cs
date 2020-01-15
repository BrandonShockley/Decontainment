using Asm;
using UnityEngine;

namespace Editor
{
    public class Divider : MonoBehaviour
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