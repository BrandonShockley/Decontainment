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

    private Instruction instruction;

    private Image bg;
    private TextMeshProUGUI tm;

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
        }
    }

    void Awake()
    {
        bg = GetComponent<Image>();
        tm = GetComponentInChildren<TextMeshProUGUI>();
    }
}
