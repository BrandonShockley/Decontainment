using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    private Transform[] playerSpawns = new Transform[3];
    [SerializeField]
    private Transform[] enemySpawns = new Transform[3];
    [SerializeField]
    private GameObject botPrefab = null;

    void Start()
    {
        LevelData levelData = LevelManager.Instance.levelData;

        // Spawn enemies
        {
            BotData[] botDatas = levelData.TeamData.BotDatas;
            for (int i = 0; i < botDatas.Length; ++i) {
                if (botDatas[i] != null) {
                    Controller.CreateBot(botPrefab, botDatas[i], 1, enemySpawns[i].position, Vector2.down);
                }
            }
        }

        // Spawn player bots
        {
            BotData[] botDatas = LevelManager.Instance.playerTeamData.BotDatas;
            for (int i = 0; i < botDatas.Length; ++i) {
                if (botDatas[i] != null) {
                    Controller.CreateBot(botPrefab, botDatas[i], 0, playerSpawns[i].position, Vector2.up);
                }
            }
        }
    }
}
