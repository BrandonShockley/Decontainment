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
        private Transform codeBlockParent = null;
        [SerializeField]
        private Transform instructionPointer = null;

        private Transform[] codeBlockTransforms;
        private List<RectTransform> slotRTs = new List<RectTransform>();

        private Canvas canvas;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        void Start()
        {
            Program program = controller.vm.program;

            // Create code block for each instruction in program
            codeBlockTransforms = new Transform[program.instructions.Length];
            int lineNumber = 0;
            int nextLabelIndex = 0;
            foreach (Instruction instruction in program.instructions) {
                // Create any labels for the current line
                while (nextLabelIndex < program.branchLabelList.Count && program.branchLabelList[nextLabelIndex].val == lineNumber) {
                    GameObject labelBlock = Instantiate(labelBlockPrefab, codeBlockParent, false);
                    string labelText = program.branchLabelList[nextLabelIndex].name + ": " + lineNumber;
                    labelBlock.GetComponentInChildren<TextMeshProUGUI>().text = labelText;
                    ++nextLabelIndex;
                }

                // Create code block
                GameObject codeBlock = Instantiate(codeBlockPrefab, codeBlockParent, false);
                codeBlock.GetComponent<CodeBlock>().Init(instruction, slotRTs);
                codeBlockTransforms[lineNumber] = codeBlock.transform;
                ++lineNumber;
            }

            HandleTick();
            controller.vm.OnTick += HandleTick;
        }

        private void HandleTick()
        {
            // Update instruction pointer
            Transform codeBlock = codeBlockTransforms[controller.vm.pc];
            instructionPointer.SetParent(codeBlock, false);
        }
    }
}
