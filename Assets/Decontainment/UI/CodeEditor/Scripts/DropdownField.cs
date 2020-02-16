using Asm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class DropdownField : MonoBehaviour
    {
        public void Init(ArgumentSpec argSpec, Argument arg)
        {
            // Configure dropdown options
            TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
            TextMeshProUGUI tm = GetComponentInChildren<TextMeshProUGUI>();

            float maxPreferredWidth = 0;
            if (argSpec.regOnly) {
                for (int regNum = 0; regNum < VirtualMachine.NUM_REGS; ++regNum) {
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
            dropdown.value = arg.val;

            // Register value change handler
            Argument.Type argType = argSpec.regOnly ? Argument.Type.REGISTER : Argument.Type.IMMEDIATE;
            dropdown.onValueChanged.AddListener((int val) =>
            {
                arg.val = val;
            });

            // Resize to fit the max preferred width
            RectTransform dropdownRT = dropdown.GetComponent<RectTransform>();
            RectTransform labelRT = tm.GetComponent<RectTransform>();
            dropdownRT.sizeDelta = new Vector2(maxPreferredWidth - labelRT.sizeDelta.x, dropdownRT.sizeDelta.y);
        }
    }
}