using Bot;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Bot 
{
    public class ProjectilePreview : MonoBehaviour 
    {

        [Serializable]
        private class Entry 
        {
            public WeaponData weapon = null;
            public GameObject preview = null;
        }

        [SerializeField]
        private GameObject botSelectorGO = null;

        [SerializeField]
        private Entry[] entries = new Entry[4];

        private Dictionary<WeaponData, GameObject> map = new Dictionary<WeaponData, GameObject>();

        private BotData currentBot;

        private IBotSelector botSelector;

        void Awake() 
        {
            botSelector = botSelectorGO.GetComponent<IBotSelector>();

            botSelector.OnBotSelected += HandleBotSelected;
        }

        void Start() 
        {
            foreach (Entry e in entries) {
                map.Add(e.weapon, e.preview);
            }
            HandleBotSelected();
        }

        void OnDestroy() 
        {
            if (currentBot != null) {
                currentBot.OnWeaponChange -= HandleWeaponChanged;
            }
        }

        private void HandleBotSelected() 
        {
            if (currentBot != null) {
                currentBot.OnWeaponChange -= HandleWeaponChanged;
            }

            currentBot = botSelector.CurrentBot;

            if (currentBot != null) {
                currentBot.OnWeaponChange += HandleWeaponChanged;
            }
            HandleWeaponChanged();
        }

        private void HandleWeaponChanged() 
        {
            ClearProjectileImage();
            if (currentBot != null && currentBot.WeaponData != null) {
                foreach (KeyValuePair<WeaponData, GameObject> entry in map) {
                    if (currentBot.WeaponData == entry.Key) {
                        entry.Value.SetActive(true);
                        break;
                    }
                }
            }
        }

        private void ClearProjectileImage() 
        {
            foreach (KeyValuePair<WeaponData, GameObject> entry in map) {
                entry.Value.SetActive(false);
            }
        }
    }
}
