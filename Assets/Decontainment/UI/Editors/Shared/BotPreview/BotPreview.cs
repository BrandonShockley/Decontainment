using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Bot
{
    public class BotPreview : MonoBehaviour
    {
        [SerializeField]
        private GameObject botSelectorGO = null;
        [SerializeField]
        private Image hardpointImage = null;

        private BotData currentBot;

        private IBotSelector botSelector;
        private Image botImage;

        void Awake()
        {
            botSelector = botSelectorGO.GetComponent<IBotSelector>();
            botImage = GetComponent<Image>();

            botSelector.OnBotSelected += HandleBotSelected;
        }

        void Start()
        {
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
            if (currentBot == null) {
                botImage.enabled = false;
                hardpointImage.enabled = false;
            } else {
                botImage.enabled = true;

                if (currentBot.WeaponData == null) {
                    hardpointImage.enabled = false;
                } else {
                    hardpointImage.enabled = true;
                    hardpointImage.color = currentBot.WeaponData.hardpointColor;
                }
            }
        }
    }
}
