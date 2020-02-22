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

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        image.enabled = false;
        text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter >= 0)
        {
            counter -= Time.unscaledDeltaTime;
        }
        if (counter < 0)
        {
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
