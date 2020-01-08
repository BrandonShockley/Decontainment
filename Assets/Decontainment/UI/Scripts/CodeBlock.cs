using Asm;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeBlock : MonoBehaviour
{
    [SerializeField]
    private OpCategoryColorMap opCategoryColorMap = null;
    [SerializeField]
    private ArgTokenColorMap argTokenColorMap = null;
    [SerializeField]
    private GameObject dropdownFieldPrefab = null;
    [SerializeField]
    private GameObject slotFieldPrefab = null;
    [SerializeField]
    private GameObject tokenPrefab = null;
    [SerializeField]
    private GameObject headerPrefab = null;

    private Instruction instruction;

    private Image bg;
    private TextMeshProUGUI opTM;
    private Transform contentParent;

    public Instruction Instruction
    {
        get { return instruction; }
        set {
            instruction = value;

            // Configure text
            opTM.text = instruction.opCode.ToString();

            // Configure color
            OpCategory category = InstructionMaps.opCategoryMap[instruction.opCode];
            bg.color = opCategoryColorMap.map[category];

            // Configure argument fields
            ArgumentSpec[] argSpecs = InstructionMaps.opArgSpecMap[instruction.opCode];
            for (int argNum = 0; argNum < instruction.args.Length; ++argNum) {
                GameObject field;
                Argument arg = instruction.args[argNum];
                if (argSpecs[argNum].regOnly || argSpecs[argNum].presets != null) {
                    // Dropdown field
                    field = Instantiate(dropdownFieldPrefab, Vector3.zero, Quaternion.identity, contentParent);

                    // Configure dropdown options
                    TMP_Dropdown dropdown = field.GetComponent<TMP_Dropdown>();
                    TextMeshProUGUI tm = field.GetComponentInChildren<TextMeshProUGUI>();

                    float maxPreferredWidth = 0;
                    if (argSpecs[argNum].regOnly) {
                        for (int regNum = 0; regNum < VirtualMachine.NUM_REGS; ++regNum) {
                            string regName = "R" + regNum;
                            dropdown.options.Add(new TMP_Dropdown.OptionData(regName));
                            maxPreferredWidth = Mathf.Max(tm.GetPreferredValues(regName).x, maxPreferredWidth);
                        }
                    } else {
                        foreach (string macroName in argSpecs[argNum].presets) {
                            dropdown.options.Add(new TMP_Dropdown.OptionData(macroName));
                            maxPreferredWidth = Mathf.Max(tm.GetPreferredValues(macroName).x, maxPreferredWidth);
                        }
                    }
                    dropdown.value = arg.val;

                    // Resize to fit the max preferred width
                    RectTransform dropdownRT = dropdown.GetComponent<RectTransform>();
                    RectTransform labelRT = tm.GetComponent<RectTransform>();
                    dropdownRT.sizeDelta = new Vector2(maxPreferredWidth - labelRT.sizeDelta.x, dropdownRT.sizeDelta.y);
                } else {
                    // Slot field
                    field = Instantiate(slotFieldPrefab, Vector3.zero, Quaternion.identity, contentParent);

                    if (arg.type == Argument.Type.REGISTER || arg.type == Argument.Type.LABEL) {
                        // Insert token into slot
                        GameObject token = Instantiate(tokenPrefab, field.transform, false);

                        // Configure color
                        // TODO: This big ternary statement is confusing
                        ArgTokenColorMap.Type tokenType = arg.type == Argument.Type.REGISTER
                            ? ArgTokenColorMap.Type.REGISTER
                            : arg.label.type == Label.Type.BRANCH
                            ? ArgTokenColorMap.Type.BRANCH_LABEL
                            : ArgTokenColorMap.Type.CONST_LABEL;
                        token.GetComponent<Image>().color = argTokenColorMap.map[tokenType];

                        // Configure text
                        TextMeshProUGUI tm = token.GetComponentInChildren<TextMeshProUGUI>();
                        if (arg.type == Argument.Type.REGISTER) {
                            tm.text = "R" + arg.val.ToString();
                        } else {
                            tm.text = arg.label.name;
                        }

                        // Resize to fit the preferred width
                        RectTransform rt = field.GetComponent<RectTransform>();
                        rt.sizeDelta = new Vector2(tm.GetPreferredValues(tm.text).x, rt.sizeDelta.y);
                    } else {
                        field.GetComponent<TMP_InputField>().text = arg.val.ToString();
                    }
                }

                // Add header
                GameObject header = Instantiate(headerPrefab, field.transform, false);
                header.GetComponent<TextMeshProUGUI>().text = argSpecs[argNum].name;
            }
        }
    }

    void Awake()
    {
        bg = GetComponent<Image>();
        opTM = GetComponentInChildren<TextMeshProUGUI>();
        contentParent = GetComponentInChildren<HorizontalLayoutGroup>().transform;
    }
}
