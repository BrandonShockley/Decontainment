using Editor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapList : ReadOnlyEditorList<GameObject>
{
    protected override void InitList()
    {
        GameObject[] mapPrefabs = Resources.LoadAll<GameObject>("MapPrefabs");
        items.AddRange(mapPrefabs);
    }

    protected override void CreateListEntry(GameObject mapPrefab, int siblingIndex = -1)
    {
        GameObject mapPreviewGO = Instantiate(listEntryPrefab, transform);
        MapPreview mapPreview = mapPreviewGO.GetComponent<MapPreview>();
        mapPreview.Init(mapPrefab, Resources.Load<Sprite>("MapPreviews/" + mapPrefab.name));
        mapPreview.OnSelect += () => HandleSelect(mapPreview.transform.GetSiblingIndex());
    }
}