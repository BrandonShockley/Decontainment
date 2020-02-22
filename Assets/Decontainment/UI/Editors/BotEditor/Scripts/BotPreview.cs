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
        private BotConfiguration botConfiguration = null;
        [SerializeField]
        private Image hardpointImage = null;
        [SerializeField]
        private Image projectileImage = null;

        private Image botImage;

        void Awake()
        {
            botImage = GetComponent<Image>();

            botConfiguration.OnBotSelected += HandleBotSelected;
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

            if (botConfiguration.CurrentBot != null) {
                botConfiguration.CurrentBot.OnWeaponChange += HandleWeaponChanged;
            }
            HandleWeaponChanged();
        }

        private void HandleWeaponChanged()
        {
            if (botConfiguration.CurrentBot == null) {
                botImage.enabled = false;
                hardpointImage.enabled = false;
                projectileImage.enabled = false;
            } else {
                botImage.enabled = true;

                if (botConfiguration.CurrentBot.WeaponData == null) {
                    hardpointImage.enabled = false;
                    projectileImage.enabled = false;
                } else {
                    hardpointImage.enabled = true;
                    hardpointImage.color = botConfiguration.CurrentBot.WeaponData.hardpointColor;
                    projectileImage.enabled = false;
                    projectileImage.color = botConfiguration.CurrentBot.WeaponData.projectilePrefab.GetComponent<LineRenderer>();
                }
            }
        }
    }
}
