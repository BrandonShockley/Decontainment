using Asm;
using System.Collections;
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
        private TextMeshProUGUI tm;

        void Awake()
        {
            image = GetComponent<Image>();
            tm = GetComponentInChildren<TextMeshProUGUI>();

            unselectedColor = image.color;
        }

        public void Init(Program program, CodeList codeList)
        {
            this.program = program;
            this.codeList = codeList;
            tm.text = program.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (codeList.Program != program) {
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