using Bot;
using System;

namespace Editor
{
    public interface IBotSelector
    {
        event Action<BotData> OnBotSelected;
        BotData CurrentBot { get; }
    }
}