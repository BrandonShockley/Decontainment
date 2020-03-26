using Bot;
using Editor.Team;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchStart : MonoBehaviour
{
    [SerializeField]
    private MapList mapList = null;
    [SerializeField]
    private TeamList teamList1 = null;
    [SerializeField]
    private TeamList teamList2 = null;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        teamList1.OnItemSelected += HandleSelected;
        teamList2.OnItemSelected += HandleSelected;
        mapList.OnItemSelected += HandleSelected;
    }

    public void StartMatch()
    {
        MatchData.Instance.mapPrefab = mapList.SelectedItem;
        MatchData.Instance.teamDatas[0] = teamList1.SelectedItem;
        MatchData.Instance.teamDatas[1] = teamList2.SelectedItem;
        SceneManager.LoadScene("Arena");
    }

    private void HandleSelected(int prevIndex)
    {
        bool team1Selected = teamList1.SelectedItem != null;
        bool team2Selected = teamList2.SelectedItem != null;
        bool mapSelected = mapList.SelectedItem != null;

        button.interactable = team1Selected && team2Selected && mapSelected;
    }
}
