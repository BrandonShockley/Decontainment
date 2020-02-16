using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor
{
    public class LabelBlock : Block
    {
        private Label label;

        public void Init(Label label, Divider myDivider)
        {
            base.Init(myDivider);
            this.label = label;

            string labelText = label.name + " (" + label.val + ")";
            GetComponentInChildren<TextMeshProUGUI>().text = labelText;

            Draggable draggable = GetComponent<Draggable>();
            draggable.onDragSuccess = Move;
            draggable.onDragTrash = (Draggable.Slot slot) =>
            {
                Globals.program.RemoveLabel(label);
                Destroy(gameObject);
            };
        }

        private void Move(Draggable.Slot slot)
        {
            Divider targetDivider = (Divider)slot;

            int oldLineNumber = label.val;
            int newLineNumber = targetDivider.lineNumber;

            label.val = newLineNumber;

            // Remove old entry
            Globals.program.branchLabelList.Remove(label);

            // Find new entry
            int insertionIndex = 0;
            for (int i = 0; i < Globals.program.branchLabelList.Count; ++i) {
                Label l = Globals.program.branchLabelList[i];
                if (l.val > label.val) {
                    break;
                } else if (l.val < label.val) {
                    insertionIndex = i + 1;
                } else if (l.val == label.val) {
                    if (l == targetDivider.label) {
                        insertionIndex = i;
                        break;
                    } else {
                        insertionIndex = i + 1;
                    }
                }
            }

            Globals.program.branchLabelList.Insert(insertionIndex, label);
            Globals.program.BroadcastBranchLabelChange();

            // Reset frontend
            Destroy(gameObject);
        }
    }
}