using Asm;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// TODO: Add a .Program subnamspace to Editor
namespace Editor
{
    public class ListEntry : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Color selectedColor = Color.white;

        private Program program;
        private CodeList codeList;
        private Color unselectedColor;

        private Image image;
        private Renamable rn;
        private TMP_InputField inputField;

        void Awake()
        {
            image = GetComponent<Image>();
            rn = GetComponent<Renamable>();
            inputField = GetComponent<TMP_InputField>();

            unselectedColor = image.color;
        }

        public void Init(Program program, CodeList codeList, Action<Program, int, string> handleRename)
        {
            this.program = program;
            this.codeList = codeList;
            rn.OnRename += (string name) => handleRename(program, transform.GetSiblingIndex(), name);
            inputField.text = program.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && codeList.Program != program) {
                image.color = selectedColor;
                codeList.Program = program;
                codeList.OnProgramChange += HandleProgramChange;
            }
        }

        private void HandleProgramChange(Program oldProgram)
        {
            if (codeList.Program != program) {
                image.color = unselectedColor;
                codeList.OnProgramChange -= HandleProgramChange;
            }
        }
    }
}