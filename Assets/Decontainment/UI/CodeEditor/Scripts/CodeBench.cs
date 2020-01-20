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
        private Transform instructionList = null;
        [SerializeField]
        private Transform tokenList = null;
        [SerializeField]
        private ColorChangeSlot trashSlot = null;
        [SerializeField]
        private GameObject instructionBlockPrefab = null;
        [SerializeField]
        private GameObject tokenPrefab = null;
        [SerializeField]
        private GameObject constTokenContainerPrefab = null;
        [SerializeField]
        private GameObject headerPrefab = null;

        private GameObject[] labelTokens;

        void Start()
        {
            Globals.trashSlots.Add(trashSlot);

            // Create an instruction block for each opcode
            for (OpCategory opCategory = 0; opCategory < OpCategory._SIZE; ++opCategory) {
                GameObject header = Instantiate(headerPrefab, instructionList, false);
                header.GetComponent<TextMeshProUGUI>().text = opCategory.ToString();

                List<OpCode> opCodes = InstructionMaps.opCategoryOpCodesMap[opCategory];
                foreach (OpCode opCode in opCodes) {
                    Instruction instruction = new Instruction(opCode);
                    CreateInstructionBlock(instruction);
                }
            }

            // Create token for each register
            GameObject regHeader = Instantiate(headerPrefab, tokenList, false);
            regHeader.GetComponent<TextMeshProUGUI>().text = "REGISTERS";

            for (int regNum = 0; regNum < VirtualMachine.NUM_REGS; ++regNum) {
                Argument arg = new Argument(Argument.Type.REGISTER, regNum);
                CreateToken(arg, tokenList);
            }

            // Create token for each branch label
            GameObject branchLabelHeader = Instantiate(headerPrefab, tokenList, false);
            branchLabelHeader.GetComponent<TextMeshProUGUI>().text = "BRANCH_LABELS";

            foreach (Label label in Globals.program.branchLabelList) {
                Argument arg = new Argument(Argument.Type.LABEL, label);
                CreateToken(arg, tokenList);
            }

            // Create token container for each const label
            GameObject constLabelHeader = Instantiate(headerPrefab, tokenList, false);
            constLabelHeader.GetComponent<TextMeshProUGUI>().text = "CONSTANT_LABELS";

            foreach (Label label in Globals.program.constLabelList) {
                Transform constTokenContainer = Instantiate(constTokenContainerPrefab, tokenList).transform;
                Argument arg = new Argument(Argument.Type.LABEL, label);
                Token token = CreateToken(arg, constTokenContainer);
                token.transform.SetAsFirstSibling();
            }
        }

        private InstructionBlock CreateInstructionBlock(Instruction instruction, InstructionBlock originalBlock = null)
        {
            InstructionBlock instructionBlock = Instantiate(instructionBlockPrefab, instructionList, false).GetComponent<InstructionBlock>();
            instructionBlock.Init(-1, instruction, null);

            if (originalBlock != null) {
                instructionBlock.transform.SetSiblingIndex(originalBlock.transform.GetSiblingIndex());
            }

            // Create a clone to take its place upon being dragged
            Draggable draggable = instructionBlock.GetComponent<Draggable>();
            Action oldOnDragStart = draggable.onDragStart;
            draggable.onDragStart = () =>
            {
                InstructionBlock clone = CreateInstructionBlock(instruction, instructionBlock);
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
            token.Init(arg, true);

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
                oldOnDragSuccess?.Invoke(slot);
            };

            return token;
        }
    }
}