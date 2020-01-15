using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// NOTE: The editor system is built on the assumption that it is
// the only thing that can modify the in-memory instruction
//
// If this changes, we will need to setup on-change events within
// the instruction/argument classes and make handlers for them here
namespace Editor
{
    public class CodeBlock : Block
    {
        [SerializeField]
        private OpCategoryColorMap opCategoryColorMap = null;
        [SerializeField]
        private GameObject dropdownFieldPrefab = null;
        [SerializeField]
        private GameObject slotFieldPrefab = null;
        [SerializeField]
        private GameObject headerPrefab = null;

        private TextMeshProUGUI opCodeTM;

        new void Awake()
        {
            base.Awake();
            opCodeTM = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Init(Instruction instruction, List<RectTransform> slotRTs, List<RectTransform> dividerRTs, RectTransform myDivider, Action<RectTransform> onDragSuccess)
        {
            base.Init(dividerRTs, myDivider, onDragSuccess);

            // Configure text
            opCodeTM.text = instruction.opCode.ToString();

            // Configure color
            OpCategory category = InstructionMaps.opCategoryMap[instruction.opCode];
            bg.color = opCategoryColorMap.map[category];

            // Create argument fields
            ArgumentSpec[] argSpecs = InstructionMaps.opArgSpecMap[instruction.opCode];
            for (int argNum = 0; argNum < instruction.args.Length; ++argNum) {
                GameObject field;
                Argument arg = instruction.args[argNum];

                if (argSpecs[argNum].regOnly || argSpecs[argNum].presets != null) {
                    field = Instantiate(dropdownFieldPrefab, transform);
                    field.GetComponent<DropdownField>().Init(argSpecs[argNum], arg);
                } else {
                    field = Instantiate(slotFieldPrefab, transform);
                    field.GetComponent<SlotField>().Init(arg, slotRTs);
                }

                // Add header
                GameObject header = Instantiate(headerPrefab, field.transform);
                header.GetComponent<TextMeshProUGUI>().text = argSpecs[argNum].name;
            }
        }
    }
}