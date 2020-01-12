using Asm;
using Bot;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor
{
    public class CodeView : MonoBehaviour
    {
        public Controller controller;

        [SerializeField]
        private GameObject codeBlockPrefab = null;
        [SerializeField]
        private GameObject labelBlockPrefab = null;
        [SerializeField]
        private GameObject dividerPrefab = null;
        [SerializeField]
        private Transform codeBlockParent = null;
        [SerializeField]
        private Transform instructionPointer = null;

        private Transform[] codeBlockTransforms;

        private Canvas canvas;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        void Start()
        {
            List<RectTransform> dividerRTs = new List<RectTransform>();
            List<RectTransform> slotRTs = new List<RectTransform>();
            Program program = controller.vm.program;

            // Create code block for each instruction in program
            codeBlockTransforms = new Transform[program.instructions.Count];
            int lineNumber = 0;
            int nextLabelIndex = 0;
            foreach (Instruction instruction in program.instructions) {
                int lineNumberCopy = lineNumber;
                // Create any labels for the current line
                while (nextLabelIndex < program.branchLabelList.Count && program.branchLabelList[nextLabelIndex].val == lineNumber) {
                    GameObject labelDivider = Instantiate(dividerPrefab, codeBlockParent, false);

                    // TODO: The line number for a label will change if a new instruction is inserted above it
                    // Thus it should probably be its own script that subscribes to changes in the program order
                    GameObject labelBlock = Instantiate(labelBlockPrefab, codeBlockParent, false);
                    string labelText = program.branchLabelList[nextLabelIndex].name + " (" + lineNumber + ")";
                    labelBlock.GetComponentInChildren<TextMeshProUGUI>().text = labelText;
                    ++nextLabelIndex;
                }

                // Create divider
                GameObject divider = Instantiate(dividerPrefab, codeBlockParent, false);
                dividerRTs.Add(divider.GetComponent<RectTransform>());

                // Create code block
                GameObject codeBlock = Instantiate(codeBlockPrefab, codeBlockParent, false);
                codeBlock.GetComponent<CodeBlock>().Init(lineNumber, instruction, slotRTs, dividerRTs,
                    (RectTransform rt) =>
                    {
                        // TODO: Store the line number with the divider so we don't have to linear search
                        // Modify backend
                        // Instruction list
                        int newLineNumber = dividerRTs.IndexOf(rt);
                        program.instructions.RemoveAt(lineNumberCopy);
                        int adjustedNewLineNumber = newLineNumber > lineNumberCopy ? newLineNumber - 1 : newLineNumber;
                        program.instructions.Insert(adjustedNewLineNumber, instruction);

                        // Label records
                        foreach (Label label in program.labelMap.Values) {
                            if (label.type == Label.Type.BRANCH) {
                                if (label.val > lineNumberCopy && label.val < newLineNumber) {
                                    --label.val;
                                } else if (label.val < lineNumberCopy && label.val > newLineNumber) {
                                    ++label.val;
                                }
                            }
                        }

                        // Reset frontend
                        Destroy(codeBlock);
                        Reset();
                    }
                );
                codeBlockTransforms[lineNumber] = codeBlock.transform;
                ++lineNumber;
            }

            // Create end divider
            GameObject endDivider = Instantiate(dividerPrefab, codeBlockParent, false);
            dividerRTs.Add(endDivider.GetComponent<RectTransform>());

            HandleTick();
            controller.vm.OnTick += HandleTick;
        }

        void OnDestroy()
        {
            controller.vm.OnTick -= HandleTick;
        }

        private void Reset()
        {
            instructionPointer.SetParent(canvas.transform, false);
            for (int i = codeBlockParent.childCount - 1; i >= 0; --i) {
                Destroy(codeBlockParent.GetChild(i).gameObject);
            }
            Start();
        }

        private void HandleTick()
        {
            // Update instruction pointer
            Transform codeBlock = codeBlockTransforms[controller.vm.pc];
            instructionPointer.SetParent(codeBlock, false);
        }
    }
}
