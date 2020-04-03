using Asm;
using UnityEngine;
using UnityEngine.Profiling;

namespace Editor.Code
{
    public class SelectionDragContainer : MonoBehaviour
    {
        private CodeList codeList;
        private Draggable.State[] oldChildStates;
        private int startBlockIndex;
        private int blockLength;
        private int numLines;

        private Draggable draggable;

        void Awake()
        {
            draggable = GetComponent<Draggable>();
        }

        public void Init(CodeList codeList, int startBlockIndex, int blockLength, int numLines)
        {
            this.codeList = codeList;
            this.startBlockIndex = startBlockIndex;
            this.blockLength = blockLength;
            this.numLines = numLines;

            // Save states
            oldChildStates = new Draggable.State[blockLength];
            for (int i = startBlockIndex; i < startBlockIndex + blockLength; ++i) {
                RectTransform blockRT = codeList.Blocks[i].GetComponent<RectTransform>();
                oldChildStates[i - startBlockIndex].Save(blockRT);
            }

            // Parent the children
            for (int i = startBlockIndex; i < startBlockIndex + blockLength; ++i) {
                codeList.Blocks[i].PreDrag();
                RectTransform blockRT = codeList.Blocks[i].GetComponent<RectTransform>();
                blockRT.SetParent(transform, false);
            }

            draggable.Init(codeList.Dividers, codeList.TrashSlots);
            draggable.onDragSuccess = Move;
            draggable.onDragTrash = Delete;
            draggable.onDragCancel = Restore;
        }

        private void Move(Draggable.Slot slot)
        {
            Divider divider = (Divider) slot;

            codeList.SelectionManager.MoveSelection(divider);
            codeList.Program.BroadcastMultiChange(new Program.Change(){ instruction = true, branchLabel = true });
            Destroy(gameObject);
        }

        private void Delete(Draggable.Slot slot)
        {
            codeList.SelectionManager.DeleteSelection();
            codeList.Program.BroadcastMultiChange(new Program.Change(){ instruction = true, branchLabel = true });
            Destroy(gameObject);
        }

        private void Restore()
        {
            // Restore blocks and delete self
            for (int i = startBlockIndex; i < startBlockIndex + blockLength; ++i) {
                RectTransform blockRT = codeList.Blocks[i].GetComponent<RectTransform>();
                oldChildStates[i - startBlockIndex].RestorePosition(blockRT);
                oldChildStates[i - startBlockIndex].RestoreParent(blockRT);
                codeList.Blocks[i].PostDrag();
            }

            Destroy(gameObject);
        }
    }
}