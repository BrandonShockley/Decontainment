using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Random = UnityEngine.Random;

namespace Match
{
    public class DemoMatchManager : SceneSingleton<DemoMatchManager>
    {
        [SerializeField]
        private float matchChangeInterval = 15.0f;

        private float changeTimer;
        private GameObject map;
        private List<TeamData> teams;
        private GameObject[] mapPrefabs;

        void Start()
        {
            Time.timeScale = 1;
            teams = new List<TeamData>(Resources.LoadAll<TeamData>(TeamDirectory.RESOURCES_PATH));
            for (int i = teams.Count - 1; i >= 0; --i) {
                if (!teams[i].Demoable) {
                    teams.RemoveAt(i);
                }
            }
            mapPrefabs = Resources.LoadAll<GameObject>(Map.MAP_PREFABS_DIR);

            ResetMatch();
            BotManager.Instance.OnTeamDisable += (teamId) => ResetMatch();
        }

        void Update()
        {
            changeTimer += Time.unscaledDeltaTime;
            if (changeTimer >= matchChangeInterval) {
                ResetMatch();
            }
        }

        private void ResetMatch()
        {
            changeTimer = 0;
            if (map != null) {
                Destroy(map);
                BotManager.Instance.ClearAll();
            }

            // Choose random builtin team
            MatchData.Instance.teamDatas[0] = teams[Random.Range(0, teams.Count)];
            MatchData.Instance.teamDatas[1] = teams[Random.Range(0, teams.Count)];

            // Choose random map
            MatchData.Instance.mapPrefab = mapPrefabs[Random.Range(0, mapPrefabs.Length)];
            map = Instantiate(MatchData.Instance.mapPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}