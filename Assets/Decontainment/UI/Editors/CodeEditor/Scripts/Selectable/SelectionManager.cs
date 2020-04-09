using Asm;
using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;

namespace Editor.Code
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject selectionDragContainerPrefab = null;

        private Divider selectedDivider;
        private int baseSelectionIndex = -1;
        private int secondSelectionIndex = -1;

        private CodeList codeList;
        private int copySourceLine;
        private List<Instruction> copiedInstructions = new List<Instruction>();
        private List<Label> copiedBranchLabels = new List<Label>();

        public bool RegionSelected => baseSelectionIndex != -1;
        public bool Pastable => RegionSelected || selectedDivider != null;

        void Awake()
        {
            codeList = GetComponent<CodeList>();
        }

        public void Reset()
        {
            baseSelectionIndex = -1;
            secondSelectionIndex = -1;
            selectedDivider = null;
        }

        public void OnSelectableClicked(Selectable selectable, bool shiftClicked)
        {
            if (selectable.TryGetComponent<Divider>(out Divider divider)) {
                // Clear stuff for divider mode
                if (selectedDivider != null) {
                    selectedDivider.GetComponent<Selectable>().Deselect();
                } else if (baseSelectionIndex != -1) {
                    ForEachSelectedBlock((b) => b.GetComponent<Selectable>().Deselect());
                    baseSelectionIndex = -1;
                }

                if (selectedDivider == divider) {
                    selectedDivider = null;
                } else {
                    selectedDivider = divider;
                    selectedDivider.GetComponent<Selectable>().Select();
                }
            } else if (selectable.TryGetComponent<Block>(out Block block)) {
                // NOTE: This linear search is not great
                int clickedIndex = codeList.Blocks.FindIndex((b) => b.GetComponent<Selectable>() == selectable);
                if (clickedIndex == -1) {
                    // This block is in the code bench
                    return;
                }

                // Clear stuff for block mode
                if (selectedDivider != null) {
                    selectedDivider.GetComponent<Selectable>().Deselect();
                    selectedDivider = null;
                } else if (baseSelectionIndex != -1) {
                    ForEachSelectedBlock((b) => b.GetComponent<Selectable>().Deselect());
                }

                if (shiftClicked) {
                    secondSelectionIndex = clickedIndex;
                } else if (clickedIndex == baseSelectionIndex) {
                    baseSelectionIndex = -1;
                } else {
                    baseSelectionIndex = clickedIndex;
                    secondSelectionIndex = clickedIndex;
                }

                ForEachSelectedBlock((b) => b.GetComponent<Selectable>().Select());
            }
        }

        public void OnSelectableDraggableDragged(PointerEventData eventData)
        {
            GameObject selectionDragContainer = Instantiate(selectionDragContainerPrefab, transform);
            int selectionStartIndex = Math.Min(baseSelectionIndex, secondSelectionIndex);
            int selectionLength = Math.Abs(baseSelectionIndex - secondSelectionIndex) + 1;
            PreprocessSelection(out int startLineNumber, out int numLines, out List<Label> labels);

            selectionDragContainer.GetComponent<SelectionDragContainer>().Init(codeList, selectionStartIndex, selectionLength, numLines);
            eventData.pointerDrag = selectionDragContainer;
            ExecuteEvents.Execute(selectionDragContainer, eventData, ExecuteEvents.beginDragHandler);
            ExecuteEvents.Execute(selectionDragContainer, eventData, ExecuteEvents.initializePotentialDrag);

        }

        public void DeleteSelection()
        {
            PreprocessSelection(out int startLineNumber, out int numLines, out List<Label> labels);

            // Remove labels
            if (labels.Count != 0) {
                labels.ForEach((label) => codeList.Program.RemoveLabel(label));
            }

            // Remove instructions
            if (startLineNumber != -1) {
                codeList.Program.instructions.RemoveRange(startLineNumber, numLines);

                // Adjust labels
                foreach (Label label in codeList.Program.branchLabelList) {
                    if (label.val >= startLineNumber + numLines) {
                        label.val -= numLines;
                    }
                }
            }

            baseSelectionIndex = -1;
        }

        public void CopySelection()
        {
            PreprocessSelection(out int startLineNumber, out int numLines, out copiedBranchLabels);

            if (startLineNumber != -1) {
                copiedInstructions.Clear();
                foreach (Instruction i in codeList.Program.instructions.GetRange(startLineNumber, numLines)) {
                    copiedInstructions.Add(i.ShallowCopy());
                }
                copySourceLine = startLineNumber;
            } else {
                copySourceLine = copiedBranchLabels[0].val;
            }
        }

        public void MoveSelection(Divider targetDivider)
        {
            PreprocessSelection(out int selectionLineNumber, out int numLines, out List<Label> movedBranchLabels);
            int insertionLineNumber = targetDivider.lineNumber;
            if (insertionLineNumber > selectionLineNumber) {
                insertionLineNumber = insertionLineNumber - numLines;
            }

            // Move instructions
            if (selectionLineNumber != -1) {
                // Copy
                List<Instruction> movedInstructions = new List<Instruction>();
                foreach (Instruction i in codeList.Program.instructions.GetRange(selectionLineNumber, numLines)) {
                    movedInstructions.Add(i);
                }

                // Delete
                codeList.Program.instructions.RemoveRange(selectionLineNumber, numLines);

                // Insert
                codeList.Program.instructions.InsertRange(insertionLineNumber, movedInstructions);
            }


            // Move labels

            codeList.Program.branchLabelList.RemoveAll((l) => movedBranchLabels.Contains(l));

            // Change moved label values
            foreach (Label label in movedBranchLabels) {
                if (selectionLineNumber == -1) {
                    label.val = insertionLineNumber;
                } else {
                    label.val = label.val - selectionLineNumber + insertionLineNumber;
                }
            }

            if (selectionLineNumber != -1) {
                // Change non-moved label values
                bool crossed = false;
                foreach (Label label in codeList.Program.branchLabelList) {
                    if (targetDivider.label == label) {
                        crossed = true; // Crossed => Insertion is before current label; !crossed doesn't guarantee the opposite
                    }

                    bool selectedBefore = selectionLineNumber < label.val;
                    bool insertedAfter = !crossed && targetDivider.lineNumber >= label.val;

                    if (selectedBefore && insertedAfter) {
                        label.val -= numLines;
                    } else if (!selectedBefore && !insertedAfter) {
                        label.val += numLines;
                    }
                }
            }

            // Insert moved labels back into list
            InsertBranchLabels(movedBranchLabels, targetDivider);
        }

        /// Pastes at targetDivider or selection if targetDivider not given
        public void PasteBuffer(Divider targetDivider = null)
        {
            // Make a shallow copy of the copied instructions in case we paste again later
            List<Instruction> copiedInstructionsCopy = new List<Instruction>();
            foreach (Instruction i in copiedInstructions) {
                copiedInstructionsCopy.Add(i.ShallowCopy());
            }

            int numLines = 0;
            HashSet<Label> selectedLabels = new HashSet<Label>();
            if (targetDivider == null) {
                if (selectedDivider != null) {
                    targetDivider = selectedDivider;
                } else {
                    int minIndex = Math.Min(baseSelectionIndex, secondSelectionIndex);
                    targetDivider = codeList.Blocks[minIndex].MyDivider;
                    PreprocessSelection(out int startLineNumber, out numLines, out List<Label> selectedLabelList);
                    selectedLabels.UnionWith(selectedLabelList);
                }
            }
            int pasteLine = targetDivider.lineNumber;
            int pasteLength = copiedInstructionsCopy.Count;

            // Adjust pre-existing branch labels
            bool crossed = false;
            for (int i = 0; i < codeList.Program.branchLabelList.Count; ++i) {
                Label label = codeList.Program.branchLabelList[i];
                if (targetDivider.label == label) {
                    crossed = true;
                }

                if (selectedLabels.Contains(label)) {
                    // Label will be overwritten
                    if (copiedBranchLabels.Contains(label)) {
                        // Will be replaced => remove but do not remove references
                        codeList.Program.labelMap.Remove(label.name);
                        codeList.Program.branchLabelList.RemoveAt(i);
                    } else {
                        // Will not be replaced => remove along with references in both buffer and program
                        codeList.Program.RemoveLabel(label);

                        foreach (Instruction instruction in copiedInstructionsCopy) {
                            foreach (Argument arg in instruction.args) {
                                if (arg.type == Argument.Type.LABEL && arg.label == label) {
                                    arg.label = null;
                                    arg.type = Argument.Type.IMMEDIATE;
                                    arg.val = 0;
                                }
                            }
                        }
                    }
                    --i;
                } else if (crossed || label.val > pasteLine) {
                    // Will not be overwritten => move
                    label.val += pasteLength - numLines;
                }
            }

            // Adjust copied branch labels
            List<Label> copiedBranchLabelsCopy = new List<Label>();
            foreach (Label label in copiedBranchLabels) {
                Label newLabel;
                if (codeList.Program.labelMap.ContainsKey(label.name)) {
                    // Labels that would've been overwritten are already removed, so we know this is one that will stay
                    // Must create a new label and adjust copied references to point to it
                    string newName = GenerateNewLabelName(label.name);
                    newLabel = new Label(newName, label.val - copySourceLine + pasteLine, label.type);

                    foreach (Instruction i in copiedInstructionsCopy) {
                        foreach (Argument arg in i.args) {
                            if (arg.type == Argument.Type.LABEL && arg.label == label) {
                                arg.label = newLabel;
                                arg.val = newLabel.val;
                            }
                        }
                    }
                } else {
                    // Copied labels that don't exist anymore should be adjusted and readded
                    int newLabelVal = label.val - copySourceLine + pasteLine;
                    newLabel = new Label(label.name, newLabelVal, label.type);
                }

                copiedBranchLabelsCopy.Add(newLabel);
                codeList.Program.labelMap.Add(newLabel.name, newLabel);
            }
            // Insert into program list
            InsertBranchLabels(copiedBranchLabelsCopy, targetDivider);

            // Recreate copied const labels if needed
            foreach (Instruction i in copiedInstructions) {
                foreach (Argument arg in i.args) {
                    if (arg.type == Argument.Type.LABEL && arg.label.type == Label.Type.CONST) {
                        if (codeList.Program.labelMap.TryGetValue(arg.label.name, out Label label)) {
                            if (label.type != Label.Type.CONST) {
                                // Recreate with generated name
                                arg.label.name = GenerateNewLabelName(arg.label.name);
                                codeList.Program.constLabelList.InsertAlphabetically(arg.label);
                                codeList.Program.labelMap.Add(arg.label.name, arg.label);
                            }
                        } else {
                            // Recreate with same name
                            codeList.Program.constLabelList.InsertAlphabetically(arg.label);
                            codeList.Program.labelMap.Add(arg.label.name, arg.label);
                        }
                    }
                }
            }

            // Replace instructions
            codeList.Program.instructions.RemoveRange(pasteLine, numLines);
            codeList.Program.instructions.InsertRange(pasteLine, copiedInstructionsCopy);
        }

        private string GenerateNewLabelName(string baseName)
        {
            string newName;
            for (int i = 0;; ++i) {
                newName = baseName + i.ToString();
                if (!codeList.Program.labelMap.ContainsKey(newName)) {
                    return newName;
                }
            }
        }

        /// Find starting line number, ending line number, and a list of labels included in selection
        /// If no instructions selected, startLineNumber = -1
        private void PreprocessSelection(out int startLineNumber, out int numLines, out List<Label> labels)
        {
            startLineNumber = -1;
            int endLineNumber = -1;
            int startIndex = Math.Min(baseSelectionIndex, secondSelectionIndex);
            int length = Math.Abs(baseSelectionIndex - secondSelectionIndex) + 1;

            labels = new List<Label>();
            for (int i = startIndex; i < startIndex + length; ++i) {
                if (codeList.Blocks[i].MyDivider.label != null) {
                    labels.Add(codeList.Blocks[i].MyDivider.label);
                } else {
                    int lineNumber = codeList.Blocks[i].MyDivider.lineNumber;
                    if (startLineNumber == -1) {
                        startLineNumber = lineNumber;
                    }
                    endLineNumber = lineNumber;
                }
            }

            if (startLineNumber == -1) {
                numLines = 0;
            } else {
                numLines = endLineNumber - startLineNumber + 1;
            }
        }

        private void InsertBranchLabels(List<Label> branchLabels, Divider targetDivider)
        {
            int newLabelIndex = 0;
            for (int labelIndex = 0; labelIndex < codeList.Program.branchLabelList.Count && newLabelIndex < branchLabels.Count; ++labelIndex) {
                Label label = codeList.Program.branchLabelList[labelIndex];
                Label movedLabel = branchLabels[newLabelIndex];
                if (label.val > movedLabel.val || (label.val == movedLabel.val && label == targetDivider.label)) {
                    codeList.Program.branchLabelList.Insert(labelIndex, movedLabel);
                    ++newLabelIndex;
                }
            }

            // Add any remaining moved labels to back of list
            for (; newLabelIndex < branchLabels.Count; ++newLabelIndex) {
                codeList.Program.branchLabelList.Add(branchLabels[newLabelIndex]);
            }
        }

        private void ForEachSelectedBlock(Action<Block> action)
        {
            if (baseSelectionIndex == -1) {
                return;
            }

            int startIndex = Math.Min(baseSelectionIndex, secondSelectionIndex);
            int length = Math.Abs(secondSelectionIndex - baseSelectionIndex) + 1;
            for (int i = startIndex; i < startIndex + length; ++i) {
                action(codeList.Blocks[i]);
            }
        }
    }
}