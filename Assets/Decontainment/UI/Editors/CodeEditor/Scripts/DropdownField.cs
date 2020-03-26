using Asm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Code
{
    public class DropdownField : MonoBehaviour
    {
        private Argument arg;
        private Trigger selfChange;

        private TMP_Dropdown dropdown;

        void OnDestroy()
        {
            if (arg != null) {
                arg.OnChange -= HandleArgChange;
            }
        }

        public void Init(ArgumentSpec argSpec, Argument arg, CodeList codeList)
        {
            this.arg = arg;

            // Configure dropdown options
            dropdown = GetComponent<TMP_Dropdown>();
            TextMeshProUGUI tm = GetComponentInChildren<TextMeshProUGUI>();

            float maxPreferredWidth = 0;
            if (argSpec.regOnly) {
                for (int regNum = 0; regNum < VirtualMachine.NUM_TOTAL_REGS; ++regNum) {
                    string regName = "R" + regNum;
                    dropdown.options.Add(new TMP_Dropdown.OptionData(regName));
                    maxPreferredWidth = Mathf.Max(tm.GetPreferredValues(regName).x, maxPreferredWidth);
                }
            } else {
                foreach (string presetName in argSpec.presets) {
                    dropdown.options.Add(new TMP_Dropdown.OptionData(presetName));
                    maxPreferredWidth = Mathf.Max(tm.GetPreferredValues(presetName).x, maxPreferredWidth);
                }
            }

            // Register value change handlers
            Argument.Type argType = argSpec.regOnly ? Argument.Type.REGISTER : Argument.Type.IMMEDIATE;
            dropdown.onValueChanged.AddListener((int val) =>
            {
                if (selfChange.Value) {
                    return;
                }
                arg.val = val;
                selfChange.Value = true;
                arg.BroadcastChange();
                codeList.Program.BroadcastArgumentChange();
            });
            arg.OnChange += HandleArgChange;

            // Init value
            HandleArgChange();

            // Resize to fit the max preferred width
            RectTransform dropdownRT = dropdown.GetComponent<RectTransform>();
            RectTransform labelRT = tm.GetComponent<RectTransform>();
            dropdownRT.sizeDelta = new Vector2(maxPreferredWidth - labelRT.sizeDelta.x, dropdownRT.sizeDelta.y);
        }

        private void HandleArgChange()
        {
            if (selfChange.Value) {
                return;
            }

            if (dropdown.value != arg.val) {
                selfChange.Value = true;
                dropdown.value = arg.val;
            }
        }
    }
}