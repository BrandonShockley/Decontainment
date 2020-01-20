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
    public class CodeList : MonoBehaviour
    {
        public Controller controller;

        [SerializeField]
        private GameObject instructionBlockPrefab = null;
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

        private bool initialCGBlocksRaycasts;
        private Transform[] instructionBlockTransforms;

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

            Globals.Init(controller.vm.program);
            controller.vm.OnTick += HandleTick;
            initialCGBlocksRaycasts = cg.blocksRaycasts;

            Globals.program.OnInstructionChange += Reset;
            Globals.program.OnBranchLabelChange += Reset;
            Globals.program.OnConstLabelChange += Reset;
        }

        void Start()
        {
            cg.blocksRaycasts = initialCGBlocksRaycasts;

            Globals.Reset();
            Program program = Globals.program;

            // Create code block for each instruction in program
            instructionBlockTransforms = new Transform[program.instructions.Count];
            int lineNumber = 0;
            int nextLabelIndex = 0;
            foreach (Instruction instruction in program.instructions) {
                // Create any labels for the current line
                while (nextLabelIndex < program.branchLabelList.Count && program.branchLabelList[nextLabelIndex].val == lineNumber) {
                    CreateLabel(ref nextLabelIndex);
                }

                // Create divider
                Divider divider = Instantiate(dividerPrefab, transform, false).GetComponent<Divider>();
                divider.Init(lineNumber);
                Globals.dividers.Add(divider);

                // Create code block
                GameObject instructionBlock = Instantiate(instructionBlockPrefab, transform, false);
                instructionBlock.GetComponent<InstructionBlock>().Init(lineNumber, instruction, divider);

                instructionBlockTransforms[lineNumber] = instructionBlock.transform;
                ++lineNumber;
            }

            // Create any remaining labels
            while (nextLabelIndex < program.branchLabelList.Count) {
                CreateLabel(ref nextLabelIndex);
            }

            // Create end divider
            Divider endDivider = Instantiate(dividerPrefab, transform, false).GetComponent<Divider>();
            endDivider.Init(lineNumber);
            Globals.dividers.Add(endDivider);

            OnRectTransformDimensionsChange();
            HandleTick();
        }

        void OnDestroy()
        {
            controller.vm.OnTick -= HandleTick;
        }

        // Ensure that code blocks are always at least as wide as the viewport
        void OnRectTransformDimensionsChange()
        {
            if (instructionBlockTransforms == null) {
                return;
            }

            float maxInstructionBlockWidth = 0;
            foreach (Transform instructionBlock in instructionBlockTransforms) {
                RectTransform instructionBlockRT = instructionBlock.GetComponent<RectTransform>();
                maxInstructionBlockWidth = Mathf.Max(instructionBlockRT.GetWorldSize().x, maxInstructionBlockWidth);
            }

            if (viewportRT.GetWorldSize().x < maxInstructionBlockWidth) {
                csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            } else {
                csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);
            }
        }

        public void Reset()
        {
            for (int i = transform.childCount - 1; i >= 0; --i) {
                Destroy(transform.GetChild(i).gameObject);
            }
            Start();
        }

        private void CreateLabel(ref int nextLabelIndex)
        {
            Program program = controller.vm.program;
            Label label = program.branchLabelList[nextLabelIndex];

            Divider labelDivider = Instantiate(dividerPrefab, transform, false).GetComponent<Divider>();
            labelDivider.Init(label.val, label);
            Globals.dividers.Add(labelDivider);

            // TODO: Need to test on bad devices to see if there's a performance hit when the code list is reset
            // NOTE: Maybe if there is, we can use pooling and continue to do a full reset
            GameObject labelBlock = Instantiate(labelBlockPrefab, transform, false);
            labelBlock.GetComponent<LabelBlock>().Init(label, labelDivider);
            ++nextLabelIndex;
        }

        private void HandleTick()
        {
            // Position instruction pointer
            RectTransform instructionBlockRT = instructionBlockTransforms[controller.vm.pc].GetComponent<RectTransform>();
            Vector2 oldPos = instructionPointerRT.position;
            Vector2 newPos = new Vector2(oldPos.x, instructionBlockRT.position.y);
            instructionPointerRT.position = newPos;
        }

        private void HandleScroll(Vector2 pos)
        {
            HandleTick();
        }
    }
}
