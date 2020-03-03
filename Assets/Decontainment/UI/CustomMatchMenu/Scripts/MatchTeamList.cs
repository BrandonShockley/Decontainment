using Editor;
using Editor.Team;
using Extensions;
using UnityEngine;

public class MatchTeamList : TeamList
{
    [SerializeField]
    private GameObject builtInListEntryPrefab = null;

    private int firstBuiltInIndex;

    protected override void InitList()
    {
        base.InitList();

        firstBuiltInIndex = Count;

        foreach (TeamData teamData in Resources.LoadAll<TeamData>(TeamDirectory.RESOURCES_PATH)) {
            TeamData copy = teamData.Copy();
            copy.name += " (Built-In)";
            items.Add(copy);
        }
    }

    // NOTE: This solution is a bit hacky.
    // If we ever need to reorder stuff in the MatchTeamList, this will probably break.
    protected override void CreateListEntry(TeamData item, int siblingIndex = -1)
    {
        GameObject listEntry;
        if (transform.childCount >= firstBuiltInIndex) {
           listEntry = Instantiate(builtInListEntryPrefab, transform);
        } else {
           listEntry = Instantiate(listEntryPrefab, transform);
        }
        listEntry.GetComponent<ListEntry>().Init(item.ToString(), HandleSelect, HandleRename);
    }
}