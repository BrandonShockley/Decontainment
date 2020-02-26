using Asm;
using Bot;
using Editor.Code;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor.Team
{
    public class MemberConfiguration : MonoBehaviour//, IBotSelector
    {
        [SerializeField]
        private int index;
        [SerializeField]
        private TeamList teamList;

        private TMP_Dropdown dropdown;

        public event Action<BotData> OnBotSelected;

        // public BotData CurrentBot
        // {
        //     get { return teamList.SelectedItem.BotDatas[index]; }
        //     set {
        //         BotData oldBot = _currentBot;
        //         _currentBot = value;

        //         OnBotSelected?.Invoke(oldBot);
        //     }
        // }

        // void Awake()
        // {
        //     dropdown = GetComponent<TMP_Dropdown>();

        //     teamList.OnItemSelected += HandleTeamSelected;
        // }

        // private void HandleTeamSelected()
        // {
        //     dropdown.value =
        // }

        private void HandleBotAdded(int index)
        {
            dropdown.options.RemoveAt(index);
        }
    }
}
