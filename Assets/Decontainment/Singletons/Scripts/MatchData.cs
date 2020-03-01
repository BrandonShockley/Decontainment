using UnityEngine;

public class MatchData : PersistentSingleton<MatchData>
{
    public GameObject mapPrefab;
    public TeamData[] teamDatas = new TeamData[2];
}