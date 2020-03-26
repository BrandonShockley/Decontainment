using System;

namespace Editor
{
    public interface IListEntry
    {
        event Action OnSelect;

        void Select();
        void Deselect();
    }
}