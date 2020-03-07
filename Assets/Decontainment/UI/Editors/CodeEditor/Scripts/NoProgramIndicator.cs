using Asm;
using Editor;
using TMPro;
using UnityEngine;

namespace Editor.Code
{
    public class NoProgramIndicator : MonoBehaviour
    {
        [SerializeField]
        private CodeList codeList = null;

        private TextMeshProUGUI tm;

        void Awake()
        {
            tm = GetComponent<TextMeshProUGUI>();

            codeList.OnProgramChange += HandleProgramChange;
        }

        private void HandleProgramChange(Program oldProgram)
        {
            tm.enabled = codeList.Program == null;
        }
    }
}