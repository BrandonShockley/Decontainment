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
            for (int i = 0; i < levelData.TeamData.BotCount; ++i) {
                Controller.CreateBot(botPrefab, levelData.TeamData.GetBotData(i), 1, enemySpawns[i].position, Vector2.down);
            }
        }

        // Spawn player bots
        {
            for (int i = 0; i < levelData.TeamData.BotCount; ++i) {
                Controller.CreateBot(botPrefab, LevelManager.Instance.playerTeamData.GetBotData(i), 0, playerSpawns[i].position, Vector2.up);
            }
        }
    }
}
