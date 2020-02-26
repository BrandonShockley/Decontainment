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
        private BotList botList = null;

        private Trigger selfChange;
        private WeaponData[] weaponDatas;

        private TMP_Dropdown dropdown;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();

            weaponDatas = Resources.LoadAll<WeaponData>(WeaponData.RESOURCES_DIR);

            botList.OnItemSelected += HandleBotSelected;

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
                    botList.SelectedItem.WeaponData = null;
                } else {
                    botList.SelectedItem.WeaponData = weaponDatas[val];
                }
            });
        }

        void Start()
        {
            HandleBotSelected(-1);
        }

        private void HandleBotSelected(int oldIndex)
        {
            BotData oldItem = botList[oldIndex];
            if (oldItem != null) {
                oldItem.OnWeaponChange -= HandleWeaponChanged;
            }

            if (botList.SelectedItem == null) {
                dropdown.interactable = false;
            } else {
                dropdown.interactable = true;
                botList.SelectedItem.OnWeaponChange += HandleWeaponChanged;
            }

            HandleWeaponChanged();
        }

        private void HandleWeaponChanged()
        {
            if (selfChange.Value) {
                return;
            }

            int newValue;
            if (botList.SelectedItem == null) {
                newValue = dropdown.options.Count - 1;
            } else {
                int index = Array.FindIndex(weaponDatas, (WeaponData weaponData) => weaponData == botList.SelectedItem.WeaponData);

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