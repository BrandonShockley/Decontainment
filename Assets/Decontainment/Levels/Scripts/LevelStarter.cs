using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    void Start()
    {
        LevelData levelData = LevelManager.Instance.levelData;
        Instantiate(levelData.LevelPrefab, Vector3.zero, Quaternion.identity);
    }
}
