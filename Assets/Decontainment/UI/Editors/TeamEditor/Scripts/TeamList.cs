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
        [SerializeField]
        private MemberConfiguration[] memberConfigurations = new MemberConfiguration[TeamData.TEAM_SIZE];

        protected override string DefaultName { get { return "Team"; } }

        protected override void SubAwake()
        {
            for (int i = 0; i < memberConfigurations.Length; ++i) {
                memberConfigurations[i].OnBotSelected += (BotData botData) => HandleBotSelected(i);
            }
        }

        protected override void InitList()
        {
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

        protected override void SubHandleSelect()
        {
            if (SelectedItem == null) {
                foreach (MemberConfiguration mc in memberConfigurations) {
                    mc.CurrentBot = null;
                }
            } else {
                for (int i = 0; i < SelectedItem.BotCount; ++i) {
                    MemberConfiguration mc = memberConfigurations[i];
                    BotData bot = SelectedItem.BotDatas[i];
                    if (bot == null) {
                        mc.CurrentBot = null;
                    } else {
                        mc.CurrentBot = botList.Find(bot.name);
                    }
                }
            }
        }

        private void HandleBotDeleted(int index, string botName)
        {
            foreach (TeamData teamData in items) {
                for (int i = 0; i < teamData.BotCount; ++i) {
                    if (teamData.GetBotName(i) == botName) {
                        teamData.SetBotName(i, null);
                    }
                }
            }
        }

        private void HandleBotRenamed(string oldName, int oldIndex, int newIndex)
        {
            // The program references are automatically updated for Assets in the editor
            #if !UNITY_EDITOR
            string newName = botList.Index(newIndex).name;
            foreach (TeamData teamData in items) {
                for (int i = 0; i < teamData.BotCount; ++i) {
                    if (teamData.GetBotName(i) == oldName) {
                        teamData.SetBotName(i, newName);
                    }
                }
            }
            #endif
        }

        private void HandleBotSelected(int index)
        {
            SelectedItem.SetBotName(index, memberConfigurations[index].CurrentBot.name);
        }
    }
}