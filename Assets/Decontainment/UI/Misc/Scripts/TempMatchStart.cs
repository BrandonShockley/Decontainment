using Bot;
using Editor.Code;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempMatchStart : MonoBehaviour
{
    [SerializeField]
    private LevelData levelData = null;
    [SerializeField]
    private WeaponData playerWeapon = null;
    [SerializeField]
    private CodeList codeList = null;

    public void StartMatch()
    {
        LevelManager.Instance.levelData = levelData;
        BotData playerBotData = BotData.CreateNew(codeList.Program.name, playerWeapon);
        LevelManager.Instance.playerTeamData = TeamData.CreateNew(new BotData[] { playerBotData, playerBotData, playerBotData });
        SceneManager.LoadScene("Arena");
    }
}
