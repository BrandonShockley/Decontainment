using Asm;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeBlock : MonoBehaviour
{
    [SerializeField]
    private OpCategoryColorMap OpCategoryColorMap = null;
    [SerializeField]
    private GameObject argDropdown = null;
    [SerializeField]
    private GameObject argSlot = null;

    private Instruction instruction;

    private Image bg;
    private TextMeshProUGUI tm;
    private Transform contentParent;

    public Instruction Instruction
    {
        get { return instruction; }
        set {
            instruction = value;

            // Configure text
            string text = instruction.opCode.ToString();
            foreach (Argument arg in instruction.args) {
                text += " ";
                if (arg.isReg) {
                    text += "R";
                }
                text += arg.val.ToString();
            }
            tm.text = text;

            // Configure color
            OpCategory category = InstructionMaps.opCategoryMap[instruction.opCode];
            bg.color = OpCategoryColorMap.map[category];

            // Configure argument fields
            ArgumentMeta[] argMetas = InstructionMaps.opArgMetaMap[instruction.opCode];
            for (int argNum = 0; argNum < instruction.args.Length; ++argNum) {
                GameObject field;
                if (argMetas[argNum].regOnly || argMetas[argNum].macros != null) {
                    field = Instantiate(argDropdown, Vector3.zero, Quaternion.identity, contentParent);
                    TMP_Dropdown dropdown = field.GetComponent<TMP_Dropdown>();
                    TextMeshProUGUI tm = field.GetComponentInChildren<TextMeshProUGUI>();

                    // Configure dropdown options
                    float maxPreferredWidth = 0;
                    if (argMetas[argNum].regOnly) {
                        for (int regNum = 0; regNum < VirtualMachine.NUM_REGS; ++regNum) {
                            string regName = "R" + regNum;
                            dropdown.options.Add(new TMP_Dropdown.OptionData(regName));
                            maxPreferredWidth = Mathf.Max(tm.GetPreferredValues(regName).x, maxPreferredWidth);
                        }
                    } else {
                        foreach (string macroName in argMetas[argNum].macros) {
                            dropdown.options.Add(new TMP_Dropdown.OptionData(macroName));
                            maxPreferredWidth = Mathf.Max(tm.GetPreferredValues(macroName).x, maxPreferredWidth);
                        }
                    }
                    dropdown.value = instruction.args[argNum].val;

                    // Resize to fit the max preferred width
                    RectTransform rt = dropdown.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(maxPreferredWidth, rt.sizeDelta.y);
                } else {
                    field = Instantiate(argSlot, Vector3.zero, Quaternion.identity, contentParent);
                    field.GetComponent<TMP_InputField>().text = instruction.args[argNum].val.ToString();
                }
            }
        }
    }

    void Awake()
    {
        bg = GetComponent<Image>();
        tm = GetComponentInChildren<TextMeshProUGUI>();
        contentParent = GetComponentInChildren<HorizontalLayoutGroup>().transform;
    }
}
