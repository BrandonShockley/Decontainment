using Bot;
using System;

namespace Editor
{
    public interface IBotSelector
    {
        event Action OnBotSelected;
        BotData CurrentBot { get; }
    }
}