using Asm;
using Bot;
using System;
using TMPro;
using UnityEngine;

namespace Editor.Bot
{
    public class WeaponDropdown : MonoBehaviour
    {
        private const string NULL_STRING = "[None]";

        [SerializeField]
        private BotConfiguration botConfiguration = null;

        private Trigger selfChange;
        private WeaponData[] weaponDatas;

        private TMP_Dropdown dropdown;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();

            weaponDatas = Resources.LoadAll<WeaponData>(WeaponData.RESOURCES_DIR);

            botConfiguration.OnBotSelected += HandleBotSelected;

            foreach (WeaponData weaponData in weaponDatas) {
                dropdown.options.Add(new TMP_Dropdown.OptionData(weaponData.name));
            }
            dropdown.options.Add(new TMP_Dropdown.OptionData(NULL_STRING));

            dropdown.onValueChanged.AddListener((int val) =>
            {
                if (selfChange.Value) {
                    return;
                }

                selfChange.Value = true;
                if (val == dropdown.options.Count - 1) {
                    botConfiguration.CurrentBot.WeaponData = null;
                } else {
                    botConfiguration.CurrentBot.WeaponData = weaponDatas[val];
                }
            });
        }

        void Start()
        {
            HandleBotSelected(null);
        }

        private void HandleBotSelected(BotData oldBot)
        {
            if (oldBot != null) {
                oldBot.OnWeaponChange -= HandleWeaponChanged;
            }

            if (botConfiguration.CurrentBot == null) {
                dropdown.interactable = false;
            } else {
                dropdown.interactable = true;
                botConfiguration.CurrentBot.OnWeaponChange += HandleWeaponChanged;
            }

            HandleWeaponChanged();
        }

        private void HandleWeaponChanged()
        {
            if (selfChange.Value) {
                return;
            }

            int newValue;
            if (botConfiguration.CurrentBot == null) {
                newValue = dropdown.options.Count - 1;
            } else {
                int index = Array.FindIndex(weaponDatas, (WeaponData weaponData) => weaponData == botConfiguration.CurrentBot.WeaponData);

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
    }
}