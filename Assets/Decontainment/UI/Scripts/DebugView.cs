using Asm;
using Bot;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugView : MonoBehaviour
{
    public Controller controller;

    [SerializeField]
    private GameObject codeBlockPrefab = null;
    [SerializeField]
    private GameObject labelBlockPrefab = null;
    [SerializeField]
    private Transform codeBlockList = null;
    [SerializeField]
    private Transform instructionPointerTransform = null;

    private Transform[] codeBlockTransforms;

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
        foreach (Instruction i in program.instructions) {
            // Create any labels for the current line
            while (nextLabelIndex < program.branchLabelList.Count && program.branchLabelList[nextLabelIndex].val == lineNumber) {
                GameObject labelBlock = Instantiate(labelBlockPrefab, Vector3.zero, Quaternion.identity);
                labelBlock.transform.SetParent(codeBlockList, false);
                string labelText = program.branchLabelList[nextLabelIndex].name + ": " + lineNumber;
                labelBlock.GetComponentInChildren<TextMeshProUGUI>().text = labelText;
                ++nextLabelIndex;
            }

            // Create code block
            GameObject codeBlock = Instantiate(codeBlockPrefab, Vector3.zero, Quaternion.identity);
            codeBlock.transform.SetParent(codeBlockList, false);
            codeBlock.GetComponent<CodeBlock>().Instruction = i;
            codeBlockTransforms[lineNumber] = codeBlock.transform;
            ++lineNumber;
        }

        HandleTick();
        controller.vm.OnTick += HandleTick;
    }

    private void HandleTick()
    {
        // Update instruction pointer
        Transform codeBlockTransform = codeBlockTransforms[controller.vm.pc];
        instructionPointerTransform.SetParent(codeBlockTransform, false);
    }
}
