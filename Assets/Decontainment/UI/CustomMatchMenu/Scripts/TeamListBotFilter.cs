using Bot;
using Editor;
using Editor.Team;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamListBotFilter : MonoBehaviour, IBotSelector
{
    [SerializeField]
    private int memberIndex = 0;
    [SerializeField]
    private TeamList teamList = null;

    public BotData CurrentBot
    {
        get {
            if (teamList.SelectedItem == null) {
                return null;
            } else {
                return teamList.SelectedItem.GetBotData(memberIndex);
            }
        }
    }

    public event Action OnBotSelected;

    void Awake()
    {
        teamList.OnItemSelected += (int oldIndex) => OnBotSelected?.Invoke();
    }
}
