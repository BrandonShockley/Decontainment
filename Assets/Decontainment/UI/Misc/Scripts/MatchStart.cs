using Bot;
using Editor.Team;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchStart : MonoBehaviour
{
    [SerializeField]
    private GameObject mapPrefab = null;
    [SerializeField]
    private TeamList teamList1 = null;
    [SerializeField]
    private TeamList teamList2 = null;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        teamList1.OnItemSelected += HandleTeamSelected;
        teamList2.OnItemSelected += HandleTeamSelected;
    }

    public void StartMatch()
    {
        MatchData.Instance.mapPrefab = mapPrefab;
        MatchData.Instance.teamDatas[0] = teamList1.SelectedItem;
        MatchData.Instance.teamDatas[1] = teamList1.SelectedItem;
        SceneManager.LoadScene("Arena");
    }

    private void HandleTeamSelected(int prevIndex)
    {
        bool team1Selected = teamList1.SelectedItem != null;
        bool team2Selected = teamList2.SelectedItem != null;

        button.interactable = team1Selected && team2Selected;
    }
}
