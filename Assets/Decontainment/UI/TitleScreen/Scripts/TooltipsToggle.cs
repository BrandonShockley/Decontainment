using UnityEngine;
using UnityEngine.UI;

public class TooltipsToggle : MonoBehaviour
{
    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(HandleToggle);
        GlobalSettings.OnTooltipsEnabledChange += HandleTooltipsEnabledChange;
    }

    void OnDestroy()
    {
        GlobalSettings.OnTooltipsEnabledChange -= HandleTooltipsEnabledChange;
    }

    private void HandleToggle(bool newVal)
    {
        GlobalSettings.TooltipsEnabled = newVal;
    }

    private void HandleTooltipsEnabledChange()
    {
        toggle.isOn = GlobalSettings.TooltipsEnabled;
    }
}