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

        void Start()
        {
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
            TeamData[] teams = Resources.LoadAll<TeamData>(TeamDirectory.RESOURCES_PATH);
            MatchData.Instance.teamDatas[0] = teams[Random.Range(0, teams.Length)];
            MatchData.Instance.teamDatas[1] = teams[Random.Range(0, teams.Length)];

            // Choose random map
            GameObject[] mapPrefabs = Resources.LoadAll<GameObject>(Map.MAP_PREFABS_DIR);
            MatchData.Instance.mapPrefab = mapPrefabs[Random.Range(0, mapPrefabs.Length)];
            map = Instantiate(MatchData.Instance.mapPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}