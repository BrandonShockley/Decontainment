using Asm;
using Bot;
using Editor.Code;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor.Bot
{
    public class ProgramDropdown : DynamicAttributeDropdown<BotData, BotList, Program, ProgramList>
    {
        protected override string AttributeName { get { return targetEditorList.SelectedItem.ProgramName; } }

        protected override void ClearAttribute()
        {
            targetEditorList.SelectedItem.ProgramName = null;
        }

        protected override void SetAttribute(int index)
        {
            targetEditorList.SelectedItem.ProgramName = attributes[index].name;
        }

        protected override void RegisterChangeHandler()
        {
            currentTarget.OnProgramChange += HandleAttributeChanged;
        }

        protected override void UnregisterChangeHandler()
        {
            currentTarget.OnProgramChange -= HandleAttributeChanged;
        }
    }
}