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

    // TODO: This is a temp thing
    private IEnumerator ReturnToEditor()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene("Editor");
    }
}