using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PromptSystem : SceneSingleton<PromptSystem>
{
    private TextMeshProUGUI text;
    private Image image;
    private Banner banner;

    void Awake()
    {
        banner = FindObjectOfType<Banner>();
        text = banner.GetComponentInChildren<TextMeshProUGUI>();
        image = banner.GetComponent<Image>();
    }

    public void PromptInvalidAction(string error)
    {
        image.color = new Color(1, 0, 0, 0.25f);
        Prompt(error);
    }

    public void PromptInvalidLabelName(string newName)
    {
        switch (newName)
        {
            case "":
                PromptInvalidAction("The name of this label is blank.");
                break;
            default:
                PromptInvalidAction("The label name " + newName + " is already taken.");
                break;
        }

    }

    public void PromptOtherAction(string message)
    {
        image.color = new Color(0, 0, 1, 0.25f);
        Prompt(message);
    }

    private void Prompt(string message)
    {
        text.SetText(message);
        banner.Activate();
    }
}
