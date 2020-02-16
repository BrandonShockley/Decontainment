using Bot;
using Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPointer : MonoBehaviour
{
    [SerializeField]
    private CodeList codeList = null;
    [SerializeField]
    private Controller initialController = null;

    private Controller _controller;

    public Controller Controller
    {
        private get { return _controller; }
        set {
            if (_controller != null) {
                _controller.VM.OnTick -= Reposition;
            }

            _controller = value;

            if (_controller != null) {
                codeList.Program = _controller.VM.Program;
                _controller.VM.OnTick += Reposition;
                Reposition();
            } else {
                codeList.Program = null;
            }
        }
    }

    void OnEnable()
    {
        codeList.OnScroll += Reposition;
    }

    void OnDisable()
    {
        codeList.OnScroll -= Reposition;
    }

    void Start()
    {
        if (Controller == null) {
            Controller = initialController;
        }
    }

    private void Reposition()
    {
        RectTransform instructionBlockRT = codeList.InstructionBlockTransforms[Controller.VM.PC].GetComponent<RectTransform>();
        Vector2 oldPos = transform.position;
        Vector2 newPos = new Vector2(oldPos.x, instructionBlockRT.position.y);
        transform.position = newPos;
    }
}
