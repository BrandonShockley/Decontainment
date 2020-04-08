using Asm;
using Editor.Code;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Help
{
    public class InstructionsList : ReadOnlyEditorList<Instruction>
    {
        [SerializeField]
        private OpCategoryColorMap opCategoryColorMap = null;

        protected override void CreateListEntry(Instruction instruction, int siblingIndex = -1)
        {
            GameObject listEntryGO = Instantiate(listEntryPrefab, transform);
            TextListEntry listEntry = listEntryGO.GetComponent<TextListEntry>();
            listEntry.Init(instruction.opCode.ToString(), null);
            listEntry.OnSelect += () => HandleSelect(listEntryGO.transform.GetSiblingIndex());
            if (siblingIndex >= 0) {
                listEntry.transform.SetSiblingIndex(siblingIndex);
            }
            listEntryGO.GetComponent<Image>().color = opCategoryColorMap.map[InstructionMaps.opCodeOpCategoryMap[instruction.opCode]]; // Bit of a hack
        }

        protected override void InitList()
        {
            for (int i = 0; i < (int)OpCode._SIZE; ++i) {
                OpCode opCode = (OpCode)i;
                items.Add(new Instruction(opCode));
            }
        }
    }
}