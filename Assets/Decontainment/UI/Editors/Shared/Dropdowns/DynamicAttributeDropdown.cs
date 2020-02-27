using System;
using TMPro;
using UnityEngine;

namespace Editor
{
    /// Dynamic dropdowns can have their options changed
    /// by other editor lists
    public abstract class DynamicAttributeDropdown<T, TL, A, AL> : AttributeDropdown<T, TL, A, AL>
        where T : class // Target
        where TL : EditorList<T> // Target editor list
        where A : class // Attribute
        where AL : EditorList<A> // Attribute editor list
    {
        protected override void SubAwake()
        {
            attributes.OnItemAdded += HandleAttributeItemAdded;
            attributes.OnItemDeleted += HandleAttributeItemDeleted;
            attributes.OnItemRenamed += HandleAttributeItemRenamed;
        }
    }
}