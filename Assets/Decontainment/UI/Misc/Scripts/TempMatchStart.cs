using Bot;
using Editor.Bot;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempMatchStart : MonoBehaviour
{
    [SerializeField]
    private LevelData levelData = null;
    [SerializeField]
    private BotConfiguration botConfiguration = null;

    public void StartMatch()
    {
        LevelManager.Instance.levelData = levelData;
        LevelManager.Instance.playerTeamData = TeamData.CreateNew(
            "Super team",
            new string[] { botConfiguration.CurrentBot.name, botConfiguration.CurrentBot.name, botConfiguration.CurrentBot.name });
        SceneManager.LoadScene("Arena");
    }
}
