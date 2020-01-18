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
        private Transform tokenList = null;
        [SerializeField]
        private ColorChangeSlot trashSlot = null;
        [SerializeField]
        private GameObject instructionBlockPrefab = null;
        [SerializeField]
        private GameObject tokenPrefab = null;
        [SerializeField]
        private GameObject headerPrefab = null;

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
                CreateToken(arg);
            }
        }

        private InstructionBlock CreateInstructionBlock(Instruction instruction, InstructionBlock originalBlock = null)
        {
            InstructionBlock instructionBlock = Instantiate(instructionBlockPrefab, instructionList, false).GetComponent<InstructionBlock>();
            instructionBlock.Init(-1, instruction, null, codeList.Reset);

            if (originalBlock != null) {
                instructionBlock.transform.SetSiblingIndex(originalBlock.transform.GetSiblingIndex());
            }

            // Create a clone to take its place upon being dragged
            Draggable draggable = instructionBlock.GetComponent<Draggable>();
            draggable.onDragStart = () =>
            {
                InstructionBlock clone = CreateInstructionBlock(instruction, instructionBlock);

                // Call the base onDragStart
                instructionBlock.HandleDragStart();

                // Ensure that the clone is destroyed upon drag cancel
                draggable.onDragCancel = () => Destroy(clone.gameObject);
            };

            return instructionBlock;
        }

        private Token CreateToken(Argument arg, Token originalToken = null)
        {
            Token token = Instantiate(tokenPrefab, tokenList, false).GetComponent<Token>();
            token.Init(arg);

            if (originalToken != null) {
                token.transform.SetSiblingIndex(originalToken.transform.GetSiblingIndex());
            }

            Draggable draggable = token.GetComponent<Draggable>();
            Action oldOnDragStart = draggable.onDragStart;
            draggable.onDragStart = () =>
            {
                Token clone = CreateToken(arg, token);
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

            return token;
        }
    }
}