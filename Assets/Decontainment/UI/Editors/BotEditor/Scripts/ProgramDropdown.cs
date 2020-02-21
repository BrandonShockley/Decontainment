using Asm;
using Bot;
using Editor.Code;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Editor.Bot
{
    public class ProgramDropdown : MonoBehaviour
    {
        private const string NULL_STRING = "[None]";

        [SerializeField]
        private BotConfiguration botConfiguration = null;
        [SerializeField]
        private ProgramList programList = null;

        private Trigger selfChange;

        private TMP_Dropdown dropdown;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();

            programList.OnItemAdded += HandleProgramAdded;
            programList.OnItemDeleted += HandleProgramDeleted;
            programList.OnItemRenamed += HandleProgramRenamed;

            botConfiguration.OnBotSelected += HandleBotSelected;

            dropdown.onValueChanged.AddListener((int val) =>
            {
                if (selfChange.Value) {
                    return;
                }

                selfChange.Value = true;
                if (val == dropdown.options.Count - 1) {
                    botConfiguration.CurrentBot.ProgramName = null;
                } else {
                    botConfiguration.CurrentBot.ProgramName = dropdown.options[val].text;
                }
            });
        }

        void Start()
        {
            for (int i = 0; i < programList.Count; ++i) {
                HandleProgramAdded(i);
            }
            dropdown.options.Add(new TMP_Dropdown.OptionData(NULL_STRING));

            HandleBotSelected(null);
        }

        private void HandleBotSelected(BotData oldBot)
        {
            if (oldBot != null) {
                oldBot.OnProgramChange -= HandleProgramChanged;
            }

            if (botConfiguration.CurrentBot == null) {
                dropdown.interactable = false;
            } else {
                dropdown.interactable = true;
                botConfiguration.CurrentBot.OnProgramChange += HandleProgramChanged;
            }

            HandleProgramChanged();
        }

        private void HandleProgramChanged()
        {
            if (selfChange.Value) {
                return;
            }

            int newValue;
            if (botConfiguration.CurrentBot == null) {
                newValue = dropdown.options.Count - 1;
            } else {
                int index = dropdown.options.FindIndex(
                    (TMP_Dropdown.OptionData optionData) => botConfiguration.CurrentBot.ProgramName == optionData.text);

                if (index == -1) {
                    index = dropdown.options.Count - 1;
                }

                newValue = index;
            }

            if (newValue != dropdown.value) {
                selfChange.Value = true;
                dropdown.value = newValue;
            }
        }

        private void HandleProgramAdded(int index)
        {
            dropdown.options.Insert(index, new TMP_Dropdown.OptionData(programList.Index(index).name));
            if (index <= dropdown.value) {
                selfChange.Value = true;
                ++dropdown.value;
            }
        }

        private void HandleProgramDeleted(int index, Program program)
        {
            dropdown.options.RemoveAt(index);
            if (index < dropdown.value) {
                selfChange.Value = true;
                --dropdown.value;
            }
        }

        private void HandleProgramRenamed(string oldName, int oldIndex, int newIndex)
        {
            dropdown.options.RemoveAt(oldIndex);
            HandleProgramAdded(newIndex);
        }
    }
}