using Bot;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : SceneSingleton<MatchManager>
{
    void Awake()
    {
        BotManager.Instance.OnTeamDisable += (int teamID) =>
        {
            Debug.Log("Team " + teamID + " all disabled. Returning to editor.");
            StartCoroutine(ReturnToEditor());
        };
    }

    void Start()
    {
        Instantiate(MatchData.Instance.mapPrefab, Vector3.zero, Quaternion.identity);
    }

    // TODO: This is a temp thing
    // We should have a proper match results screen
    private IEnumerator ReturnToEditor()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene("Editor");
    }
}