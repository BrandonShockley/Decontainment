using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Banner : MonoBehaviour
{
	[SerializeField]
	private float duration = 2.5f;

    private float counter = -1.0f;
    private Image image;
    private TextMeshProUGUI text;

    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        image.enabled = false;
        text.enabled = false;
    }

    void Update()
    {
        if (counter >= 0) {
            counter -= Time.unscaledDeltaTime;
        } else {
            image.enabled = false;
            text.enabled = false;
        }
    }

    public void Activate()
    {
        counter = duration;
        image.enabled = true;
        text.enabled = true;
    }
}
