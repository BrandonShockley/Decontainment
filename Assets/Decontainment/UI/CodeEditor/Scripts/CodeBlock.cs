using Asm;
using System;
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
    public class CodeBlock : MonoBehaviour
    {
        [SerializeField]
        private float collapsedWidth = 30.0f;
        [SerializeField]
        private OpCategoryColorMap opCategoryColorMap = null;
        [SerializeField]
        private GameObject dropdownFieldPrefab = null;
        [SerializeField]
        private GameObject slotFieldPrefab = null;
        [SerializeField]
        private GameObject headerPrefab = null;

        private CanvasGroup cg;
        private Image bg;
        private RectTransform rt;
        private TextMeshProUGUI opCodeTM;
        private Transform contentParent;

        void Awake()
        {
            cg = GetComponentInParent<CanvasGroup>();
            bg = GetComponent<Image>();
            rt = GetComponent<RectTransform>();
            opCodeTM = GetComponentInChildren<TextMeshProUGUI>();
            contentParent = GetComponentInChildren<HorizontalLayoutGroup>().transform;
        }

        public void Init(int lineNumber, Instruction instruction, List<RectTransform> slotRTs, List<RectTransform> dividerRTs, RectTransform myDivider, Action<RectTransform> onDragSuccess)
        {
            Draggable draggable = GetComponent<Draggable>();
            draggable.Init(dividerRTs);
            draggable.filterFunc = (RectTransform rt) => rt == myDivider;
            draggable.onDragStart = () =>
            {
                myDivider.gameObject.SetActive(false);
                bg.raycastTarget = false;
                cg.blocksRaycasts = false;
                rt.sizeDelta = new Vector2(collapsedWidth, rt.sizeDelta.y);
            };
            draggable.onDragCancel = () =>
            {
                myDivider.gameObject.SetActive(true);
                bg.raycastTarget = true;
                cg.blocksRaycasts = true;
            };
            draggable.onDragEnter = (RectTransform rt) => rt.GetComponent<Image>().color = Color.white; // TODO: Make these parameters
            draggable.onDragExit = (RectTransform rt) => rt.GetComponent<Image>().color = Color.black;
            draggable.onDragSuccess = onDragSuccess;

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
                    field.GetComponent<SlotField>().Init(arg, slotRTs);
                }

                // Add header
                GameObject header = Instantiate(headerPrefab, field.transform, false);
                header.GetComponent<TextMeshProUGUI>().text = argSpecs[argNum].name;
            }
        }
    }
}