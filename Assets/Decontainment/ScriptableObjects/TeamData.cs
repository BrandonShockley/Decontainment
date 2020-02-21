using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObjects/TeamData", order = 1)]
public class TeamData : ScriptableObject
{
    [SerializeField]
    private BotData[] botDatas = new BotData[3];

    public BotData[] BotDatas { get { return botDatas; } }

    public static TeamData CreateNew(BotData[] botDatas)
    {
        TeamData teamData = ScriptableObject.CreateInstance<TeamData>();
        teamData.botDatas = botDatas;
        return teamData;
    }
}
