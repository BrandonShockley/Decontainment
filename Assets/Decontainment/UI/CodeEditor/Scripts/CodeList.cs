using Asm;
using Bot;
using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class CodeList : MonoBehaviour
    {
        [SerializeField]
        private GameObject instructionBlockPrefab = null;
        [SerializeField]
        private GameObject labelBlockPrefab = null;
        [SerializeField]
        private GameObject dividerPrefab = null;
        [SerializeField]
        private RectTransform viewportRT = null;
        [SerializeField]
        private ScrollRect scrollRect = null;

        private bool initialCGBlocksRaycasts;
        private Transform[] instructionBlockTransforms;

        private Program _program;
        private List<Draggable.Slot> _dividers = new List<Draggable.Slot>();
        private List<Draggable.Slot> _slotFields = new List<Draggable.Slot>();
        private List<Draggable.Slot> _trashSlots = new List<Draggable.Slot>();

        private Canvas canvas;
        private CanvasGroup cg;
        private ContentSizeFitter csf;
        private RectTransform rt;

        public event Action OnScroll;
        public event Action<Program> OnProgramChange;

        public Transform[] InstructionBlockTransforms { get { return instructionBlockTransforms; } }

        public Program Program
        {
            get { return _program; }
            set {
                Program oldProgram = _program;
                _program = value;

                if (oldProgram != null) {
                    oldProgram.OnInstructionChange -= Reset;
                    oldProgram.OnBranchLabelChange -= Reset;
                    oldProgram.OnConstLabelChange -= Reset;
                }

                Clear();
                if (_program != null) {
                    _program.OnInstructionChange += Reset;
                    _program.OnBranchLabelChange += Reset;
                    _program.OnConstLabelChange += Reset;
                    Generate();
                }

                OnProgramChange?.Invoke(oldProgram);
            }
        }
        public List<Draggable.Slot> Dividers { get { return _dividers; } }
        public List<Draggable.Slot> SlotFields { get { return _slotFields; } }
        public List<Draggable.Slot> TrashSlots { get { return _trashSlots; } }

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

            initialCGBlocksRaycasts = cg.blocksRaycasts;
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

            float viewportWidth = viewportRT.GetWorldSize().x;
            if (viewportWidth < maxInstructionBlockWidth && !Mathf.Approximately(viewportWidth, maxInstructionBlockWidth)) {
                csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            } else {
                csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);
            }
        }

        public void Reset()
        {
            Clear();
            Generate();
        }

        private void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; --i) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void Generate()
        {
            cg.blocksRaycasts = initialCGBlocksRaycasts;

            Dividers.Clear();
            SlotFields.Clear();

            // Create code block for each instruction in program
            instructionBlockTransforms = new Transform[Program.instructions.Count];
            int lineNumber = 0;
            int nextLabelIndex = 0;
            foreach (Instruction instruction in Program.instructions) {
                // Create any labels for the current line
                while (nextLabelIndex < Program.branchLabelList.Count && Program.branchLabelList[nextLabelIndex].val == lineNumber) {
                    CreateLabel(ref nextLabelIndex);
                }

                // Create divider
                Divider divider = Instantiate(dividerPrefab, transform, false).GetComponent<Divider>();
                divider.Init(lineNumber);
                Dividers.Add(divider);

                // Create code block
                GameObject instructionBlock = Instantiate(instructionBlockPrefab, transform, false);
                instructionBlock.GetComponent<InstructionBlock>().Init(lineNumber, instruction, divider, this);

                instructionBlockTransforms[lineNumber] = instructionBlock.transform;
                ++lineNumber;
            }

            // Create any remaining labels
            while (nextLabelIndex < Program.branchLabelList.Count) {
                CreateLabel(ref nextLabelIndex);
            }

            // Create end divider
            Divider endDivider = Instantiate(dividerPrefab, transform, false).GetComponent<Divider>();
            endDivider.Init(lineNumber);
            Dividers.Add(endDivider);

            OnRectTransformDimensionsChange();
        }

        private void CreateLabel(ref int nextLabelIndex)
        {
            Label label = Program.branchLabelList[nextLabelIndex];

            Divider labelDivider = Instantiate(dividerPrefab, transform, false).GetComponent<Divider>();
            labelDivider.Init(label.val, label);
            Dividers.Add(labelDivider);

            // TODO: Need to test on bad devices to see if there's a performance hit when the code list is reset
            // NOTE: Maybe if there is, we can use pooling and continue to do a full reset
            GameObject labelBlock = Instantiate(labelBlockPrefab, transform, false);
            labelBlock.GetComponent<LabelBlock>().Init(label, labelDivider, this);
            ++nextLabelIndex;
        }

        private void HandleScroll(Vector2 pos)
        {
            OnScroll?.Invoke();
        }
    }
}
