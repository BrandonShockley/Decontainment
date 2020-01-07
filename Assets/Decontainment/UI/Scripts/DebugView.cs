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
        // Create code block for each instruction in program
        codeBlockTransforms = new Transform[controller.vm.instructions.Length];
        int lineNumber = 0;
        int nextLabelIndex = 0;
        foreach (Instruction i in controller.vm.instructions) {
            // Possibly create a label first
            Tuple<string, int> nextLabel = nextLabelIndex < controller.vm.labelList.Count
                ? controller.vm.labelList[nextLabelIndex]
                : null;
            if (nextLabel != null && nextLabel.Item2 == lineNumber) {
                GameObject labelBlock = Instantiate(labelBlockPrefab, Vector3.zero, Quaternion.identity);
                labelBlock.transform.SetParent(codeBlockList, false);
                labelBlock.GetComponentInChildren<TextMeshProUGUI>().text = nextLabel.Item1;
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
