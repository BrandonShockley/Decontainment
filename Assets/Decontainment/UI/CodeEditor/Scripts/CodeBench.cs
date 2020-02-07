using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class CodeBench : MonoBehaviour
    {
        [SerializeField]
        private CodeList codeList = null;

        [SerializeField]
        private Transform instructionList = null;
        [SerializeField]
        private Transform regTokenList = null;
        [SerializeField]
        private Transform branchLabelTokenList = null;
        [SerializeField]
        private Transform constLabelTokenList = null;
        [SerializeField]
        private ColorChangeSlot trashSlot = null;
        [SerializeField]
        private GameObject instructionBlockPrefab = null;
        [SerializeField]
        private GameObject tokenPrefab = null;
        [SerializeField]
        private GameObject labelTokenContainerPrefab = null;
        [SerializeField]
        private GameObject headerPrefab = null;

        private GameObject[] labelTokens;

        void Awake()
        {
            codeList.OnProgramChange += (Program oldProgram) =>
            {
                ResetBranchLabelList();
                ResetConstLabelList();

                if (oldProgram != null) {
                    codeList.Program.OnBranchLabelChange -= ResetBranchLabelList;
                    codeList.Program.OnConstLabelChange -= ResetConstLabelList;
                }

                if (codeList.Program != null) {
                    codeList.Program.OnBranchLabelChange += ResetBranchLabelList;
                    codeList.Program.OnConstLabelChange += ResetConstLabelList;
                }
            };
        }

        void Start()
        {
            codeList.TrashSlots.Add(trashSlot);

            // Create an instruction block for each opcode
            for (OpCategory opCategory = 0; opCategory < OpCategory._SIZE; ++opCategory) {
                GameObject header = Instantiate(headerPrefab, instructionList, false);
                TextMeshProUGUI headerTM = header.GetComponent<TextMeshProUGUI>();
                RectTransform headerRT = header.GetComponent<RectTransform>();
                headerTM.text = opCategory.ToString();
                headerRT.sizeDelta = new Vector2(headerTM.preferredWidth, headerRT.sizeDelta.y);

                List<OpCode> opCodes = InstructionMaps.opCategoryOpCodesMap[opCategory];
                foreach (OpCode opCode in opCodes) {
                    CreateInstructionBlock(opCode);
                }
            }

            // Create register tokens
            for (int regNum = 0; regNum < VirtualMachine.NUM_REGS; ++regNum) {
                Argument arg = new Argument(Argument.Type.REGISTER, regNum);
                CreateToken(arg, regTokenList);
            }
        }

        // Used by buttons
        public void AddBranchLabel()
        {
            if (codeList.Program == null) {
                return;
            }

            string defaultName = "BranchLabel";
            string newName;
            for (int i = 0;; ++i) {
                newName = defaultName + i.ToString();
                if (!codeList.Program.labelMap.ContainsKey(newName)) {
                    break;
                }
            }

            Label label = new Label(newName, 0, Label.Type.BRANCH);
            codeList.Program.branchLabelList.Insert(0, label);
            codeList.Program.labelMap.Add(newName, label);
            codeList.Program.BroadcastBranchLabelChange();
        }

        // Used by buttons
        public void AddConstLabel()
        {
            if (codeList.Program == null) {
                return;
            }

            string defaultName = "ConstLabel";
            string newName;
            for (int i = 0;; ++i) {
                newName = defaultName + i.ToString();
                if (!codeList.Program.labelMap.ContainsKey(newName)) {
                    break;
                }
            }

            Label label = new Label(newName, 0, Label.Type.CONST);

            // Insert alphabetically
            List<Label> labelList = codeList.Program.constLabelList;
            int index;
            for (index = 0; index < labelList.Count; ++index) {
                if (string.Compare(label.name, labelList[index].name) <= 0) {
                    break;
                }
            }
            labelList.Insert(index, label);
            codeList.Program.labelMap.Add(newName, label);
            codeList.Program.BroadcastConstLabelChange();
        }

        private void ResetBranchLabelList()
        {
            for (int i = branchLabelTokenList.childCount - 1; i >= 0; --i) {
                Destroy(branchLabelTokenList.GetChild(i).gameObject);
            }

            if (codeList.Program != null) {
                foreach (Label label in codeList.Program.branchLabelList) {
                    Transform labelTokenContainer = Instantiate(labelTokenContainerPrefab, branchLabelTokenList).transform;
                    labelTokenContainer.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        codeList.Program.RemoveLabel(label);
                    });
                    Argument arg = new Argument(Argument.Type.LABEL, label);
                    Token token = CreateToken(arg, labelTokenContainer);
                    token.transform.SetSiblingIndex(1);
                }
            }
        }

        private void ResetConstLabelList()
        {
            for (int i = constLabelTokenList.childCount - 1; i >= 0; --i) {
                Destroy(constLabelTokenList.GetChild(i).gameObject);
            }

            if (codeList.Program != null) {
                foreach (Label label in codeList.Program.constLabelList) {
                    Transform labelTokenContainer = Instantiate(labelTokenContainerPrefab, constLabelTokenList).transform;
                    Argument arg = new Argument(Argument.Type.LABEL, label);
                    Token token = CreateToken(arg, labelTokenContainer);
                    token.transform.SetSiblingIndex(1);
                }
            }
        }

        private InstructionBlock CreateInstructionBlock(OpCode opCode, InstructionBlock originalBlock = null)
        {
            Instruction instruction = new Instruction(opCode);
            InstructionBlock instructionBlock = Instantiate(instructionBlockPrefab, instructionList, false).GetComponent<InstructionBlock>();
            instructionBlock.Init(-1, instruction, null, codeList);

            if (originalBlock != null) {
                instructionBlock.transform.SetSiblingIndex(originalBlock.transform.GetSiblingIndex());
            }

            // Create a clone to take its place upon being dragged
            Draggable draggable = instructionBlock.GetComponent<Draggable>();
            Action oldOnDragStart = draggable.onDragStart;
            draggable.onDragStart = () =>
            {
                InstructionBlock clone = CreateInstructionBlock(opCode, instructionBlock);
                oldOnDragStart?.Invoke();

                draggable.onDragCancel = () => Destroy(clone.gameObject);
            };
            Action<Draggable.Slot> oldOnDragSuccess = draggable.onDragSuccess;
            draggable.onDragSuccess = (Draggable.Slot slot) =>
            {
                draggable.onDragStart = oldOnDragStart;
                draggable.onDragSuccess = oldOnDragSuccess;
                oldOnDragSuccess?.Invoke(slot);
            };

            return instructionBlock;
        }

        private Token CreateToken(Argument arg, Transform parent, Token originalToken = null)
        {
            Token token = Instantiate(tokenPrefab, parent, false).GetComponent<Token>();
            token.Init(arg.ShallowCopy(), codeList, true);

            if (originalToken != null) {
                token.transform.SetSiblingIndex(originalToken.transform.GetSiblingIndex());
            }

            Draggable draggable = token.GetComponent<Draggable>();
            Action oldOnDragStart = draggable.onDragStart;
            draggable.onDragStart = () =>
            {
                Token clone = CreateToken(arg, parent, token);
                oldOnDragStart?.Invoke();

                draggable.onDragCancel = () => Destroy(clone.gameObject);
            };
            Action<Draggable.Slot> oldOnDragSuccess = draggable.onDragSuccess;
            draggable.onDragSuccess = (Draggable.Slot slot) =>
            {
                token.renamable = false;
                draggable.onDragStart = oldOnDragStart;
                draggable.onDragSuccess = oldOnDragSuccess;
                draggable.onDragCancel = null;
                oldOnDragSuccess?.Invoke(slot);
            };

            return token;
        }
    }
}