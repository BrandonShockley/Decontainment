using Asm;
using Bot;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private RectTransform instructionPointerRT = null;
        [SerializeField]
        private CanvasGroup instructionGroup = null;

        private List<RectTransform> dividerRTs;
        private List<RectTransform> slotRTs;
        private Transform[] codeBlockTransforms;

        private Canvas canvas;

        void OnEnable()
        {
            GetComponentInChildren<ScrollRect>().onValueChanged.AddListener(HandleScroll);
        }

        void OnDisable()
        {
            GetComponentInChildren<ScrollRect>().onValueChanged.RemoveListener(HandleScroll);
        }

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        void Start()
        {
            dividerRTs = new List<RectTransform>();
            slotRTs = new List<RectTransform>();
            Program program = controller.vm.program;

            // Create code block for each instruction in program
            codeBlockTransforms = new Transform[program.instructions.Count];
            int lineNumber = 0;
            int nextLabelIndex = 0;
            foreach (Instruction instruction in program.instructions) {
                int lineNumberCopy = lineNumber;

                // Create any labels for the current line
                while (nextLabelIndex < program.branchLabelList.Count && program.branchLabelList[nextLabelIndex].val == lineNumber) {
                    CreateLabel(ref lineNumber, ref nextLabelIndex);
                }

                // Create divider
                GameObject divider = Instantiate(dividerPrefab, codeBlockParent, false);
                divider.GetComponent<Divider>().Init(lineNumber);
                RectTransform dividerRT = divider.GetComponent<RectTransform>();
                dividerRTs.Add(dividerRT);

                // Create code block
                GameObject codeBlock = Instantiate(codeBlockPrefab, codeBlockParent, false);
                codeBlock.GetComponent<CodeBlock>().Init(lineNumber, instruction, slotRTs, dividerRTs, dividerRT,
                    (RectTransform rt) =>
                    {
                        Divider targetDivider = rt.GetComponent<Divider>();

                        // Modify backend

                        // Remove old instruction
                        int oldLineNumber = lineNumberCopy;
                        int newLineNumber = targetDivider.lineNumber;
                        program.instructions.RemoveAt(oldLineNumber);

                        // Adjust labels
                        foreach (Label label in program.branchLabelList) {
                            if (label.val > oldLineNumber) {
                                --label.val; // TODO: This will need to be variable when we add dragging selected blocks
                            }
                        }

                        // Insert new instruction
                        int adjustedNewLineNumber = newLineNumber > oldLineNumber ? newLineNumber - 1 : newLineNumber;
                        program.instructions.Insert(adjustedNewLineNumber, instruction);

                        // Adjust labels
                        bool crossed = false;
                        foreach (Label label in program.branchLabelList) {
                            if (targetDivider.label == label) {
                                crossed = true;
                            }
                            if (crossed || adjustedNewLineNumber < label.val) {
                                ++label.val; // TODO: This will need to be variable when we add dragging selected blocks
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

            // Create any remaining labels
            while (nextLabelIndex < program.branchLabelList.Count) {
                CreateLabel(ref lineNumber, ref nextLabelIndex);
            }

            // Create end divider
            GameObject endDivider = Instantiate(dividerPrefab, codeBlockParent, false);
            endDivider.GetComponent<Divider>().Init(lineNumber);
            dividerRTs.Add(endDivider.GetComponent<RectTransform>());

            HandleTick();
            controller.vm.OnTick += HandleTick;
        }

        void OnDestroy()
        {
            controller.vm.OnTick -= HandleTick;
        }

        private void CreateLabel(ref int lineNumber, ref int nextLabelIndex)
        {
            Program program = controller.vm.program;
            Label label = program.branchLabelList[nextLabelIndex];

            GameObject labelDivider = Instantiate(dividerPrefab, codeBlockParent, false);
            labelDivider.GetComponent<Divider>().Init(lineNumber, label);
            dividerRTs.Add(labelDivider.GetComponent<RectTransform>());

            // TODO: The line number for a label will change if a new instruction is inserted above it
            // Thus it should probably be its own script that subscribes to changes in the program order

            // Response: Only if we end up dynamically modifying the UI instead of doing a full reset
            GameObject labelBlock = Instantiate(labelBlockPrefab, codeBlockParent, false);
            string labelText = label.name + " (" + lineNumber + ")";
            labelBlock.GetComponentInChildren<TextMeshProUGUI>().text = labelText;
            ++nextLabelIndex;
        }

        private void Reset()
        {
            for (int i = codeBlockParent.childCount - 1; i >= 0; --i) {
                Destroy(codeBlockParent.GetChild(i).gameObject);
            }
            Start();
        }

        private void HandleTick()
        {
            // Position instruction pointer
            RectTransform codeBlockRT = codeBlockTransforms[controller.vm.pc].GetComponent<RectTransform>();
            Vector2 oldPos = instructionPointerRT.position;
            Vector2 newPos = new Vector2(oldPos.x, codeBlockRT.position.y);
            instructionPointerRT.position = newPos;
        }

        private void HandleScroll(Vector2 pos)
        {
            HandleTick();
        }
    }
}
