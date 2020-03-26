using Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BotName : MonoBehaviour
{
    [SerializeField]
    private GameObject botSelectorGO = null;

    private IBotSelector botSelector;
    private TextMeshProUGUI tm;

    void Awake()
    {
        botSelector = botSelectorGO.GetComponent<IBotSelector>();
        tm = GetComponent<TextMeshProUGUI>();

        botSelector.OnBotSelected += HandleBotSelected;
    }

    void Start()
    {
        HandleBotSelected();
    }

    private void HandleBotSelected()
    {
        if (botSelector.CurrentBot == null) {
            tm.text = "";
        } else {
            tm.text = botSelector.CurrentBot.name;
        }
    }
}
