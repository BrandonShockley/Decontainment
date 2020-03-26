using System;

public static class GlobalSettings
{
    private static bool tooltipsEnabled = true;

    public static event Action OnTooltipsEnabledChange;

    public static bool TooltipsEnabled
    {
        get => tooltipsEnabled;
        set {
            tooltipsEnabled = value;
            OnTooltipsEnabledChange?.Invoke();
        }
    }
}