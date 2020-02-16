using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Banner : MonoBehaviour
{
	[SerializeField]
	private int ticks = 5;

    private int counter = -1;
    private Image image;
    private TextMeshProUGUI text;

    protected Color defaultColor = new Color(0, 0, 0, 0);

    // Start is called before the first frame update
    void Start()
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
            image.enabled = true;
            text.enabled = true;
            counter = counter - 1;
        } else
        {
            image.enabled = false;
            text.enabled = false;
        }
    }

    public void Activate()
    {
        counter = ticks;
    }
}
