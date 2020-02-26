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
        protected override string AttributeName { get { return sourceEditorList.SelectedItem.ProgramName; } }

        protected override void ClearAttribute()
        {
            sourceEditorList.SelectedItem.ProgramName = null;
        }

        protected override void SetAttribute(int index)
        {
            sourceEditorList.SelectedItem.ProgramName = attributeEditorList[index].name;
        }

        protected override void RegisterChangeHandler(BotData botData, Action changeHandler)
        {
            botData.OnProgramChange += changeHandler;
        }

        protected override void UnregisterChangeHandler(BotData botData, Action changeHandler)
        {
            botData.OnProgramChange -= changeHandler;
        }
    }
}