using Bot;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Editor.Bot
{
    public class BotList : EditorList<BotData>
    {
        protected override string DefaultName { get { return "Bot"; } }

        protected override BotData CreateNewItem(string name)
        {
            return BotData.CreateNew(name, null, null);
        }

        protected override void DeleteItem(BotData botData)
        {
            botData.DeleteOnDisk();
        }

        protected override void InitList()
        {
            string[] filePaths = Directory.GetFiles(BotDirectory.PATH, "*" + BotDirectory.EXTENSION);
            foreach (string filePath in filePaths) {
                items.Add(BotData.Load(filePath));
            }
        }

        protected override void RenameItem(BotData item, string name)
        {
            item.Rename(name);
        }

        protected override void SubHandleSelect()
        {
            Debug.Log("this is where the fun begins");
        }
    }
}