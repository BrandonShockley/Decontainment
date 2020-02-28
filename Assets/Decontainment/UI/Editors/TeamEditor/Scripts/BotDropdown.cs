using Bot;
using Editor.Bot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Team
{
    public class BotDropdown : DynamicAttributeDropdown<TeamData, TeamList, BotData, BotList>, IBotSelector
    {
        [SerializeField]
        private int memberIndex = 0;

        public event Action OnBotSelected;

        public BotData CurrentBot {
            get {
                if (targetEditorList.SelectedItem == null) {
                    return null;
                } else {
                    return targetEditorList.SelectedItem.GetBotData(memberIndex);
                }
            }
        }

        protected override string AttributeName { get { return targetEditorList.SelectedItem.GetBotName(memberIndex); } }

        protected override void ClearAttribute()
        {
            targetEditorList.SelectedItem.SetBotName(memberIndex, null);
        }

        protected override void SetAttribute(int index)
        {
            targetEditorList.SelectedItem.SetBotName(memberIndex, attributes[index].ToString());
        }

        protected override void RegisterChangeHandler()
        {
            currentTarget.OnBotChanged += HandleBotChange;
        }

        protected override void UnregisterChangeHandler()
        {
            currentTarget.OnBotChanged -= HandleBotChange;
        }

        protected override void SubHandleTargetSelected()
        {
            HandleBotChange(memberIndex);
        }

        private void HandleBotChange(int botIndex)
        {
            if (botIndex == memberIndex) {
                HandleAttributeChanged();
                OnBotSelected?.Invoke();
            }
        }
    }
}