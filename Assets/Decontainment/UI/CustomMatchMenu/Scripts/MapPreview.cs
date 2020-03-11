using Editor;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MapPreview : MonoBehaviour, IListEntry
{
    private GameObject mapPrefab;

    private Image image;

    public event Action OnSelect;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Init(GameObject mapPrefab, Sprite mapPreview)
    {
        this.mapPrefab = mapPrefab;
        image.sprite = mapPreview;
    }

    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
    }
}