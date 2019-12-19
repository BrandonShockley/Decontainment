using Asm;
using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugView : MonoBehaviour
{
    public Controller controller;

    [SerializeField]
    private GameObject codeBlockPrefab = null;
    [SerializeField]
    private Transform codeBlockList = null;
    [SerializeField]
    private GameObject instructionPointer = null;

    void Start()
    {
        // Create code block for each instruction in program
        foreach (Instruction i in controller.vm.instructions) {
            GameObject go = Instantiate(codeBlockPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(codeBlockList, false);
            go.GetComponent<CodeBlock>().Instruction = i;
        }

        HandleTick();
        controller.vm.OnTick += HandleTick;
    }

    private void HandleTick()
    {
        instructionPointer.transform.SetParent(
            codeBlockList.GetChild(controller.vm.pc), false);
    }
}
