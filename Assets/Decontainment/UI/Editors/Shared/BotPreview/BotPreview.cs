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
            HandleBotSelected(null);
        }

        private void HandleBotSelected(BotData oldBot)
        {
            if (oldBot != null) {
                oldBot.OnWeaponChange -= HandleWeaponChanged;
            }

            if (botSelector.CurrentBot != null) {
                botSelector.CurrentBot.OnWeaponChange += HandleWeaponChanged;
            }
            HandleWeaponChanged();
        }

        private void HandleWeaponChanged()
        {
            if (botSelector.CurrentBot == null) {
                botImage.enabled = false;
                hardpointImage.enabled = false;
            } else {
                botImage.enabled = true;

                if (botSelector.CurrentBot.WeaponData == null) {
                    hardpointImage.enabled = false;
                } else {
                    hardpointImage.enabled = true;
                    hardpointImage.color = botSelector.CurrentBot.WeaponData.hardpointColor;
                }
            }
        }
    }
}
