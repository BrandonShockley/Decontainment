using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slideout : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Vector2 slideDeltaPos = Vector2.zero;
    [SerializeField]
    private float slideSpeed = 1.0f;

    private float interpolation;
    private bool hovered;
    private Vector2 initialPos;

    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        initialPos = rt.anchoredPosition;
    }

    void Update()
    {
        float deltaDistance = slideSpeed * Time.unscaledDeltaTime;
        float deltaInterpolation = deltaDistance / slideDeltaPos.magnitude;

        if (hovered) {
            interpolation = Mathf.Clamp(interpolation + deltaInterpolation, 0, 1);
        } else {
            interpolation = Mathf.Clamp(interpolation - deltaInterpolation, 0, 1);
        }

        rt.anchoredPosition = Vector2.Lerp(initialPos, initialPos + slideDeltaPos, interpolation);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
