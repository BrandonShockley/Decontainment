using Asm;
using Bot;
using System;
using TMPro;
using UnityEngine;

namespace Editor.Bot
{
    public class WeaponDropdown : AttributeDropdown<BotData, BotList, WeaponData, WeaponData[]>
    {
        protected override string AttributeName
        {
            get {
                if (targetEditorList.SelectedItem.WeaponData == null) {
                    return null;
                } else {
                    return targetEditorList.SelectedItem.WeaponData.name;
                }
            }
        }

        protected override void SubAwake()
        {
            attributes = Resources.LoadAll<WeaponData>(WeaponData.RESOURCES_DIR);
        }

        protected override void ClearAttribute()
        {
            targetEditorList.CurrentBot.WeaponData = null;
        }

        protected override void SetAttribute(int index)
        {
            targetEditorList.CurrentBot.WeaponData = attributes[index];
        }

        protected override void RegisterChangeHandler()
        {
            currentTarget.OnWeaponChange += HandleAttributeChanged;
        }

        protected override void UnregisterChangeHandler()
        {
            currentTarget.OnWeaponChange -= HandleAttributeChanged;
        }
    }
}