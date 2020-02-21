using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [SerializeField]
    private GameObject levelPrefab = null;
    [SerializeField]
    private TeamData teamData = null;

    public GameObject LevelPrefab { get { return levelPrefab; } }
    public TeamData TeamData { get { return teamData; } }
}
