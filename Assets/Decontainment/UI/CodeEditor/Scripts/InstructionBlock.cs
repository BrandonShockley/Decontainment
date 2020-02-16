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
    public class InstructionBlock : Block
    {
        [SerializeField]
        private OpCategoryColorMap opCategoryColorMap = null;
        [SerializeField]
        private GameObject dropdownFieldPrefab = null;
        [SerializeField]
        private GameObject slotFieldPrefab = null;
        [SerializeField]
        private GameObject headerPrefab = null;

        private int lineNumber;
        private Instruction instruction;

        private TextMeshProUGUI opCodeTM;

        new void Awake()
        {
            base.Awake();
            opCodeTM = GetComponentInChildren<TextMeshProUGUI>();
        }

        // lineNumber == -1 signifies no line number
        public void Init(int lineNumber, Instruction instruction, Divider myDivider)
        {
            base.Init(myDivider);
            this.lineNumber = lineNumber;
            this.instruction = instruction;

            draggable.onDragSuccess = (Draggable.Slot slot) =>
            {
                Insert(slot);
                // Reset frontend
                Destroy(gameObject);
                Globals.program.BroadcastInstructionChange();
            };
            draggable.onDragTrash = (Draggable.Slot slot) =>
            {
                if (lineNumber != -1) {
                    Remove();
                    Globals.program.BroadcastInstructionChange();
                }
                Destroy(gameObject);
            };

            // Configure text
            opCodeTM.text = instruction.opCode.ToString();

            // Configure color
            OpCategory category = InstructionMaps.opCodeOpCategoryMap[instruction.opCode];
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
                    field.GetComponent<SlotField>().Init(arg);
                }

                // Add header
                // GameObject header = Instantiate(headerPrefab, field.transform);
                // header.GetComponent<TextMeshProUGUI>().text = argSpecs[argNum].name;
            }
        }

        private void Insert(Draggable.Slot slot)
        {
            Divider targetDivider = (Divider)slot;
            int newLineNumber = targetDivider.lineNumber;
            int adjustedNewLineNumber;

            if (lineNumber != -1) {
                // Remove old instruction
                Remove();

                adjustedNewLineNumber = newLineNumber > lineNumber ? newLineNumber - 1 : newLineNumber;
            } else {
                adjustedNewLineNumber = newLineNumber;
            }

            // Insert new instruction
            Globals.program.instructions.Insert(adjustedNewLineNumber, instruction);

            // Adjust labels
            bool crossed = false;
            foreach (Label label in Globals.program.branchLabelList) {
                if (targetDivider.label == label) {
                    crossed = true;
                }
                if (crossed || adjustedNewLineNumber < label.val) {
                    ++label.val; // TODO: This will need to be variable when we add dragging selected blocks
                }
            }
        }

        private void Remove()
        {
            if (lineNumber != -1) {
                Globals.program.instructions.RemoveAt(lineNumber);

                // Adjust labels
                foreach (Label label in Globals.program.branchLabelList) {
                    if (label.val > lineNumber) {
                        --label.val; // TODO: This will need to be variable when we add dragging selected blocks
                    }
                }
            }
        }
    }
}