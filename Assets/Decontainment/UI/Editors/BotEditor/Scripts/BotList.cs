using Asm;
using Bot;
using Editor.Code;
using System;
using System.IO;
using UnityEngine;

namespace Editor.Bot
{
    public class BotList : EditorList<BotData>, IBotSelector
    {
        [SerializeField]
        private BotConfiguration botConfiguration = null;
        [SerializeField]
        private ProgramList programList = null;

        public event Action OnBotSelected;

        public BotData CurrentBot { get { return SelectedItem; } }

        protected override string DefaultName { get { return "Bot"; } }

        protected override void SubAwake()
        {
            programList.OnItemDeleted += HandleProgramDeleted;
            programList.OnItemRenamed += HandleProgramRenamed;
        }

        protected override void InitList()
        {
            if (!Directory.Exists(BotDirectory.PATH)) {
                return;
            }

            string[] filePaths = Directory.GetFiles(BotDirectory.PATH, "*" + BotDirectory.EXTENSION);
            foreach (string filePath in filePaths) {
                items.Add(BotData.Load(filePath));
            }
        }

        protected override BotData CreateNewItem(string name)
        {
            BotData botData = BotData.CreateNew(name, null, null);
            botData.Save();
            return botData;
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

        protected override void SubDelete(int oldIndex, BotData oldBot)
        {
            botConfiguration.CurrentBot = null;
            OnBotSelected?.Invoke();
        }

        protected override void SubHandleSelect(int oldIndex)
        {
            botConfiguration.CurrentBot = this[SelectedIndex];
            OnBotSelected?.Invoke();
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
            #if !UNITY_EDITOR || BUILD_MODE
            string newName = programList[newIndex].name;
            foreach (BotData botData in items) {
                if (botData.ProgramName == oldName) {
                    botData.ProgramName = newName;
                }
            }
            #endif
        }
    }
}