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
        text.SetText("NOBODY");
    }

    public void PromptInvalidAction(string error)
    {
        text.SetText("Error: Invalid Action. " + error);
        image.color = new Color(1, 0, 0, 0.25f);
        banner.Activate();
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

    public void PromptInvalidProgramName(string name)
    {
        switch (name)
        {
            case "":
                PromptInvalidAction("The name of this program is blank.");
                break;
            default:
                PromptInvalidAction("The program name " + name + " is already taken.");
                break;
        }
    }

    public void PromptOtherAction(string message)
    {
        text.SetText(message);
        image.color = new Color(0, 0, 1, 0.25f);
        banner.Activate();
    }
}
