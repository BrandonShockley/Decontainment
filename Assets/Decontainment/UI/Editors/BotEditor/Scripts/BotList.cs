using Asm;
using Bot;
using Editor.Code;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Editor.Bot
{
    public class BotList : EditorList<BotData>
    {
        [SerializeField]
        private BotConfiguration botConfiguration = null;
        [SerializeField]
        private ProgramList programList = null;

        protected override string DefaultName { get { return "Bot"; } }

        protected override void SubAwake()
        {
            programList.OnItemDeleted += HandleProgramDeleted;
            programList.OnItemRenamed += HandleProgramRenamed;
        }

        protected override void InitList()
        {
            string[] filePaths = Directory.GetFiles(BotDirectory.PATH, "*" + BotDirectory.EXTENSION);
            foreach (string filePath in filePaths) {
                items.Add(BotData.Load(filePath));
            }
        }

        protected override BotData CreateNewItem(string name)
        {
            return BotData.CreateNew(name, null, null);
        }

        protected override void DeleteItem(BotData botData)
        {
            botData.DeleteOnDisk();
            botConfiguration.CurrentBot = null;
        }

        protected override void RenameItem(BotData botData, string name)
        {
            botData.Rename(name);
        }

        protected override void SubHandleSelect()
        {
            botConfiguration.CurrentBot = items[SelectedIndex];
        }

        private void HandleProgramDeleted(int index, Program program)
        {
            foreach (BotData botData in items) {
                if (botData.ProgramName == program.name) {
                    botData.ProgramName = null;
                }
            }
        }

        private void HandleProgramRenamed(string oldName, int oldIndex, int newIndex)
        {
            // The program references are automatically updated for TextAssets in the editor
            #if !UNITY_EDITOR
            string newName = programList.Index(newIndex).name;
            foreach (BotData botData in items) {
                if (botData.ProgramName == oldName) {
                    botData.ProgramName = newName;
                }
            }
            #endif
        }
    }
}