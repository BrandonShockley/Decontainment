using Asm;
using Bot;
using Extensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class InstructionList : MonoBehaviour
    {
        public Controller controller;

        [SerializeField]
        private GameObject codeBlockPrefab = null;
        [SerializeField]
        private GameObject labelBlockPrefab = null;
        [SerializeField]
        private GameObject dividerPrefab = null;
        [SerializeField]
        private RectTransform instructionPointerRT = null;
        [SerializeField]
        private RectTransform viewportRT = null;
        [SerializeField]
        private ScrollRect scrollRect = null;

        private List<RectTransform> dividerRTs;
        private List<RectTransform> slotRTs;
        private Transform[] codeBlockTransforms;

        private Canvas canvas;
        private CanvasGroup cg;
        private ContentSizeFitter csf;
        private RectTransform rt;

        void OnEnable()
        {
            scrollRect.onValueChanged.AddListener(HandleScroll);
        }

        void OnDisable()
        {
            scrollRect.onValueChanged.RemoveListener(HandleScroll);
        }

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            cg = GetComponent<CanvasGroup>();
            csf = GetComponent<ContentSizeFitter>();
            rt = GetComponent<RectTransform>();
        }

        void Start()
        {
            cg.blocksRaycasts = true;

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
                    CreateLabel(ref nextLabelIndex);
                }

                // Create divider
                GameObject divider = Instantiate(dividerPrefab, transform, false);
                divider.GetComponent<Divider>().Init(lineNumber);
                RectTransform dividerRT = divider.GetComponent<RectTransform>();
                dividerRTs.Add(dividerRT);

                // Create code block
                GameObject codeBlock = Instantiate(codeBlockPrefab, transform, false);
                codeBlock.GetComponent<CodeBlock>().Init(instruction, slotRTs, dividerRTs, dividerRT,
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
                CreateLabel(ref nextLabelIndex);
            }

            // Create end divider
            GameObject endDivider = Instantiate(dividerPrefab, transform, false);
            endDivider.GetComponent<Divider>().Init(lineNumber);
            dividerRTs.Add(endDivider.GetComponent<RectTransform>());

            OnRectTransformDimensionsChange();
            HandleTick();
            controller.vm.OnTick += HandleTick;
        }

        void OnDestroy()
        {
            controller.vm.OnTick -= HandleTick;
        }

        // Ensure that code blocks are always at least as wide as the viewport
        void OnRectTransformDimensionsChange()
        {
            if (codeBlockTransforms == null) {
                return;
            }

            float maxCodeBlockWidth = 0;
            foreach (Transform codeBlock in codeBlockTransforms) {
                RectTransform codeBlockRT = codeBlock.GetComponent<RectTransform>();
                maxCodeBlockWidth = Mathf.Max(codeBlockRT.GetWorldSize().x, maxCodeBlockWidth);
            }

            if (viewportRT.GetWorldSize().x < maxCodeBlockWidth) {
                csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            } else {
                csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);
            }
        }

        private void CreateLabel(ref int nextLabelIndex)
        {
            Program program = controller.vm.program;
            Label label = program.branchLabelList[nextLabelIndex];

            GameObject labelDivider = Instantiate(dividerPrefab, transform, false);
            labelDivider.GetComponent<Divider>().Init(label.val, label);
            RectTransform labelDividerRT = labelDivider.GetComponent<RectTransform>();
            dividerRTs.Add(labelDividerRT);

            // TODO: The line number for a label will change if a new instruction is inserted above it
            // Thus it should probably be its own script that subscribes to changes in the program order

            // Response: Only if we end up dynamically modifying the UI instead of doing a full reset
            // TODO: Need to test on bad devices to see if there's a performance hit
            GameObject labelBlock = Instantiate(labelBlockPrefab, transform, false);
            labelBlock.GetComponent<LabelBlock>().Init(label, dividerRTs, labelDividerRT, (RectTransform rt) =>
            {
                Divider targetDivider = rt.GetComponent<Divider>();

                int oldLineNumber = label.val;
                int newLineNumber = targetDivider.lineNumber;

                label.val = newLineNumber;

                // Remove old entry
                program.branchLabelList.Remove(label);

                // Find new entry
                int insertionIndex = 0;
                for (int i = 0; i < program.branchLabelList.Count; ++i) {
                    Label l = program.branchLabelList[i];
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

                program.branchLabelList.Insert(insertionIndex, label);

                // Reset frontend
                Destroy(labelBlock);
                Reset();
            });
            ++nextLabelIndex;
        }

        private void Reset()
        {
            for (int i = transform.childCount - 1; i >= 0; --i) {
                Destroy(transform.GetChild(i).gameObject);
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
