﻿using Asm;
using Bot;
using Editor.Bot;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Editor.Team
{
    public class TeamList : EditorList<TeamData>
    {
        [SerializeField]
        private BotList botList = null;

        protected override string DefaultName { get { return "Team"; } }

        protected override void SubAwake()
        {
            if (botList != null) {
                botList.OnItemRenamed += HandleBotRenamed;
                botList.OnItemDeleted += HandleBotDeleted;
            }
        }

        protected override void InitList()
        {
            if (!Directory.Exists(TeamDirectory.PATH)) {
                return;
            }

            string[] filePaths = Directory.GetFiles(TeamDirectory.PATH, "*" + TeamDirectory.EXTENSION);
            foreach (string filePath in filePaths) {
                items.Add(TeamData.Load(filePath));
            }
        }

        protected override TeamData CreateNewItem(string name)
        {
            TeamData teamData = TeamData.CreateNew(name, new string[TeamData.TEAM_SIZE]);
            teamData.Save();
            return teamData;
        }

        protected override void DeleteItem(TeamData teamData)
        {
            teamData.DeleteOnDisk();
        }

        protected override void RenameItem(TeamData teamData, string name)
        {
            teamData.Rename(name);
        }

        private void HandleBotDeleted(int index, BotData removedBotData)
        {
            foreach (TeamData teamData in items) {
                for (int i = 0; i < teamData.BotCount; ++i) {
                    if (teamData.GetBotName(i) == removedBotData.name) {
                        teamData.SetBotName(i, null);
                    }
                }
            }
        }

        private void HandleBotRenamed(string oldName, int oldIndex, int newIndex)
        {
            // The program references are automatically updated for Assets in the editor
            #if !UNITY_EDITOR || BUILD_MODE
            string newName = botList[newIndex].name;
            foreach (TeamData teamData in items) {
                for (int i = 0; i < teamData.BotCount; ++i) {
                    if (teamData.GetBotName(i) == oldName) {
                        teamData.SetBotName(i, newName);
                    }
                }
            }
            #endif
        }
    }
}