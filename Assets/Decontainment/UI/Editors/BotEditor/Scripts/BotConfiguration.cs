using Asm;
using Bot;
using Editor.Code;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Bot
{
    public class BotConfiguration : MonoBehaviour
    {
        [SerializeField]
        private CodeList readonlyCodeList = null;
        [SerializeField]
        private ProgramList programList = null;

        private BotData _currentBot;

        public event Action<BotData> OnBotSelected;

        public BotData CurrentBot
        {
            get { return _currentBot; }
            set {
                if (_currentBot != null) {
                    _currentBot.OnProgramChange -= HandleProgramChanged;
                }

                BotData oldBot = _currentBot;
                _currentBot = value;

                if (_currentBot == null) {
                    readonlyCodeList.Program = null;
                } else {
                    _currentBot.OnProgramChange += HandleProgramChanged;
                    HandleProgramChanged();
                }
                OnBotSelected?.Invoke(oldBot);
            }
        }

        void Awake()
        {
            programList.OnItemDeleted += HandleProgramDeleted;
        }

        private void HandleProgramChanged()
        {
            readonlyCodeList.Program = programList.FindProgram(_currentBot.ProgramName);
        }

        private void HandleProgramDeleted(int index, Program program)
        {
            if (program == readonlyCodeList.Program) {
                readonlyCodeList.Program = null;
            }
        }

    }
}
