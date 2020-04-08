using System;
using TMPro;
using UnityEngine;

// From: https://forum.unity.com/threads/does-the-content-size-fitter-work.484678/
[ExecuteInEditMode]
public class TextSizer : MonoBehaviour
{
    public Vector2 padding;
    public Vector2 maxSize = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
    public Vector2 minSize;
    public Mode controlAxes = Mode.Both;

    [Flags]
    public enum Mode
    {
        None        = 0,
        Horizontal  = 0x1,
        Vertical    = 0x2,
        Both        = Horizontal | Vertical
    }

    private string lastText;
    private Mode lastControlAxes = Mode.None;
    private Vector2 lastSize;

    private RectTransform rt;
    private TMP_Text tmp;

    private float MinX => (controlAxes & Mode.Horizontal) != 0 ? minSize.x : RT.rect.width - padding.x;
    private float MinY => (controlAxes & Mode.Vertical) != 0 ? minSize.y : RT.rect.height - padding.y;
    private float MaxX => (controlAxes & Mode.Horizontal) != 0 ? maxSize.x : RT.rect.width - padding.x;
    private float MaxY => (controlAxes & Mode.Vertical) != 0 ? maxSize.y : RT.rect.height - padding.y;

    private RectTransform RT => rt != null ? rt : rt = GetComponent<RectTransform>();
    private TMP_Text TMP => tmp != null ? tmp : tmp = GetComponent<TMP_Text>();

    void Start()
    {
        Refresh();
    }

    void OnValidate()
    {
        Refresh();
    }

    void OnRectTransformDimensionsChange()
    {
        Refresh();
    }

    void Update()
    {
        if (TMP.text != lastText || controlAxes != lastControlAxes) {
            Refresh();
        }
    }

    // Forces size recalculation
    public void Refresh()
    {
        Vector2 preferredSize = TMP.GetPreferredValues();
        preferredSize.x = Mathf.Clamp(preferredSize.x, MinX, MaxX);
        preferredSize.y = Mathf.Clamp(preferredSize.y, MinY, MaxY);
        preferredSize += padding;

        if ((controlAxes & Mode.Horizontal) == 0) {
            preferredSize.x = RT.sizeDelta.x;
        }
        if ((controlAxes & Mode.Vertical) == 0) {
            preferredSize.y = RT.sizeDelta.y;
        }

        RT.sizeDelta = preferredSize;

        lastText = TMP.text;
        lastSize = RT.rect.size;
        lastControlAxes = controlAxes;
    }
}