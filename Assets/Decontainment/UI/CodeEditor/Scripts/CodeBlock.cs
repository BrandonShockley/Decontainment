using Asm;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// NOTE: The editor system is built on the assumption that it is
// the only thing that can modify the in-memory instruction
//
// If this changes, we will need to setup on-change events within
// the instruction/argument classes and make handlers for them here
namespace Editor
{
    public class CodeBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private OpCategoryColorMap opCategoryColorMap = null;
        [SerializeField]
        private GameObject dropdownFieldPrefab = null;
        [SerializeField]
        private GameObject slotFieldPrefab = null;
        [SerializeField]
        private GameObject headerPrefab = null;

        private Image bg;
        private TextMeshProUGUI opCodeTM;
        private Transform contentParent;

        void Awake()
        {
            bg = GetComponent<Image>();
            opCodeTM = GetComponentInChildren<TextMeshProUGUI>();
            contentParent = GetComponentInChildren<HorizontalLayoutGroup>().transform;
        }

        public void Init(Instruction instruction)
        {
            // Configure text
            opCodeTM.text = instruction.opCode.ToString();

            // Configure color
            OpCategory category = InstructionMaps.opCategoryMap[instruction.opCode];
            bg.color = opCategoryColorMap.map[category];

            // Create argument fields
            ArgumentSpec[] argSpecs = InstructionMaps.opArgSpecMap[instruction.opCode];
            for (int argNum = 0; argNum < instruction.args.Length; ++argNum) {
                GameObject field;
                Argument arg = instruction.args[argNum];

                if (argSpecs[argNum].regOnly || argSpecs[argNum].presets != null) {
                    field = Instantiate(dropdownFieldPrefab, Vector3.zero, Quaternion.identity, contentParent);
                    field.GetComponent<DropdownField>().Init(argSpecs[argNum], arg);
                } else {
                    field = Instantiate(slotFieldPrefab, Vector3.zero, Quaternion.identity, contentParent);
                    field.GetComponent<SlotField>().Init(arg);
                }

                // Add header
                GameObject header = Instantiate(headerPrefab, field.transform, false);
                header.GetComponent<TextMeshProUGUI>().text = argSpecs[argNum].name;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Begin drag"  + eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Drag " + eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("End drag"  + eventData.position);
        }
    }
}