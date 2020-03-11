using Bot;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private GameObject previewLabelPrefab = null;

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

    public void SpawnPreviewLabels()
    {
        for (int i = 0; i < t0Spawns.Length; ++i) {
            GameObject previewLabelGO = Instantiate(previewLabelPrefab, t0Spawns[i].position, Quaternion.identity);
            TextMeshPro tmp = previewLabelGO.GetComponent<TextMeshPro>();
            tmp.text = i.ToString();
            tmp.color = Color.cyan;
        }

        for (int i = 0; i < t1Spawns.Length; ++i) {
            GameObject previewLabelGO = Instantiate(previewLabelPrefab, t1Spawns[i].position, Quaternion.identity);
            TextMeshPro tmp = previewLabelGO.GetComponent<TextMeshPro>();
            tmp.text = i.ToString();
            tmp.color = Color.red;
        }
    }
}
