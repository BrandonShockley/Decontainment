using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health health;

    private RectTransform rt;
    private Slider s;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        s = GetComponent<Slider>();
    }

    void Update()
    {
        Rect camRect = Camera.main.rect;
        Vector2 normalizedViewPoint = Camera.main.WorldToViewportPoint(health.transform.position);
        Vector2 viewPoint = new Vector2(normalizedViewPoint.x * camRect.width, normalizedViewPoint.y * camRect.height)
            + new Vector2(camRect.xMin, camRect.yMin);
        rt.anchorMin = viewPoint;
        rt.anchorMax = viewPoint;
    }

    void OnDestroy()
    {
        health.OnHealthChange -= HandleHealthChange;
    }

    public void Init(Health health)
    {
        this.health = health;
        health.OnHealthChange += HandleHealthChange;
    }

    private void HandleHealthChange()
    {
        s.value = (float)health.Amount / health.MaxAmount;
    }
}
