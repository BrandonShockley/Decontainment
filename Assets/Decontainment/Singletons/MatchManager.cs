using Bot;
using UnityEngine;

public class MatchManager : SceneSingleton<MatchManager>
{
    void Awake()
    {
        BotManager.Instance.OnTeamDisable += (int teamID) =>
        {
            Debug.Log("Team " + teamID + " all disabled");
        };
    }
}