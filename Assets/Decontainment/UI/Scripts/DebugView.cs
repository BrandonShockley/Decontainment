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
    private RectTransform instructionPointerRT = null;

    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

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
        RectTransform codeBlockRT = codeBlockList.GetChild(controller.vm.pc).GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        codeBlockRT.GetWorldCorners(corners);
        Vector2 oldPos = instructionPointerRT.position;
        Vector2 newPos = new Vector2(oldPos.x, (corners[0].y + corners[1].y) / 2);
        instructionPointerRT.position = newPos;
        // Debug.Log(codeBlockRect.position.y);
    }
}
