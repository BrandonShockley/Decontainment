using System;
using UnityEngine;

namespace Editor.Code
{
    public class SelectionManager
    {
        private Divider selectedDivider;

        private int baseSelectionIndex = -1;
        private int secondSelectionIndex = -1;

        private CodeList codeList;

        public int BaseSelectionIndex => baseSelectionIndex;
        public int SecondSelectionIndex => secondSelectionIndex;

        public SelectionManager(CodeList codeList)
        {
            this.codeList = codeList;
        }

        public void Reset()
        {
            baseSelectionIndex = -1;
            secondSelectionIndex = -1;
            selectedDivider = null;
        }

        public void OnSelectableClicked(Selectable selectable, bool shiftClicked)
        {
            if (selectable.TryGetComponent<Divider>(out Divider divider)) {
                // Clear stuff for divider mode
                if (selectedDivider != null) {
                    selectedDivider.GetComponent<Selectable>().Deselect();
                } else if (baseSelectionIndex != -1) {
                    ForEachSelectedBlock((b) => b.GetComponent<Selectable>().Deselect());
                    baseSelectionIndex = -1;
                }

                if (selectedDivider == divider) {
                    selectedDivider = null;
                } else {
                    selectedDivider = divider;
                    selectedDivider.GetComponent<Selectable>().Select();
                }
            } else if (selectable.TryGetComponent<Block>(out Block block)) {
                // NOTE: This linear search is inefficient AF
                int clickedIndex = codeList.Blocks.FindIndex((b) => b.GetComponent<Selectable>() == selectable);
                if (clickedIndex == -1) {
                    // This block is in the code bench
                    return;
                }

                // Clear stuff for block mode
                if (selectedDivider != null) {
                    selectedDivider.GetComponent<Selectable>().Deselect();
                    selectedDivider = null;
                } else if (baseSelectionIndex != -1) {
                    ForEachSelectedBlock((b) => b.GetComponent<Selectable>().Deselect());
                }

                if (shiftClicked) {
                    secondSelectionIndex = clickedIndex;
                } else if (clickedIndex == baseSelectionIndex) {
                    baseSelectionIndex = -1;
                } else {
                    baseSelectionIndex = clickedIndex;
                    secondSelectionIndex = clickedIndex;
                }

                ForEachSelectedBlock((b) => b.GetComponent<Selectable>().Select());
            }
        }

        private void ForEachSelectedBlock(Action<Block> action)
        {
            if (baseSelectionIndex == -1) {
                return;
            }

            int startIndex = Math.Min(baseSelectionIndex, secondSelectionIndex);
            int length = Math.Abs(secondSelectionIndex - baseSelectionIndex) + 1;
            for (int i = startIndex; i < startIndex + length; ++i) {
                action(codeList.Blocks[i]);
            }
        }
    }
}