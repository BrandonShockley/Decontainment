using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Map : MonoBehaviour
{
    [SerializeField]
    private Transform[] t0Spawns = new Transform[TeamData.TEAM_SIZE];
    [SerializeField]
    private Transform[] t1Spawns = new Transform[TeamData.TEAM_SIZE];
    [SerializeField]
    private GameObject botPrefab = null;

    void Start()
    {
        #if UNITY_EDITOR && !BUILD_MODE
        AssetDatabase.Refresh();
        #endif

        // Spawn everyone
        for (int ti = 0; ti < MatchData.Instance.teamDatas.Length; ++ti) {
            TeamData teamData = MatchData.Instance.teamDatas[ti];
            for (int bi = 0; bi < teamData.BotCount; ++bi) {
                BotData botData = teamData.GetBotData(bi);
                if (botData != null) {
                    Transform spawn = ti == 0 ? t0Spawns[bi] : t1Spawns[bi];
                    Controller.CreateBot(botPrefab, botData, ti, spawn.position, spawn.rotation);
                }
            }
        }
    }
}
