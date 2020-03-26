using Editor;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapPreview : MonoBehaviour, IListEntry
{
    [SerializeField]
    private Image previewImage = null;
    [SerializeField]
    private TextMeshProUGUI nameTM = null;
    [SerializeField]
    private Sprite selectedSprite = null;

    private GameObject mapPrefab;
    private Sprite deselectedSprite;

    private Image image;

    public event Action OnSelect;

    void Awake()
    {
        image = GetComponent<Image>();

        deselectedSprite = image.sprite;
    }

    public void Init(GameObject mapPrefab, Sprite mapPreview)
    {
        this.mapPrefab = mapPrefab;
        nameTM.text = mapPrefab.name;
        previewImage.sprite = mapPreview;
    }

    public void Select()
    {
        OnSelect?.Invoke();
        image.sprite = selectedSprite;
    }

    public void Deselect()
    {
        image.sprite = deselectedSprite;
    }
}