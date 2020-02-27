using Bot;
using Editor.Team;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempMatchStart : MonoBehaviour
{
    [SerializeField]
    private LevelData levelData = null;
    [SerializeField]
    private TeamList teamList = null;

    public void StartMatch()
    {
        LevelManager.Instance.levelData = levelData;
        LevelManager.Instance.playerTeamData = teamList.SelectedItem;
        SceneManager.LoadScene("Arena");
    }
}
