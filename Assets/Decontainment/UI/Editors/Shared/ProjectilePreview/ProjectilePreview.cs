using Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Bot {
    public class ProjectilePreview : MonoBehaviour {
        [SerializeField]
        private GameObject botSelectorGO = null;
        [SerializeField]
        private Image LaserImage = null;
        [SerializeField]
        private WeaponData Laser = null;
        [SerializeField]
        private Image HealBeamImage = null;
        [SerializeField]
        private WeaponData HealBeam = null;
        [SerializeField]
        private Image PulseImage = null;
        [SerializeField]
        private WeaponData Pulse = null;
        [SerializeField]
        private GameObject BitThrowerImage = null;
        [SerializeField]
        private WeaponData BitThrower = null;

        private BotData currentBot;

        private IBotSelector botSelector;

        void Awake() {
            botSelector = botSelectorGO.GetComponent<IBotSelector>();

            botSelector.OnBotSelected += HandleBotSelected;
        }

        void Start() {
            HandleBotSelected();
        }

        void OnDestroy() {
            if (currentBot != null) {
                currentBot.OnWeaponChange -= HandleWeaponChanged;
            }
        }

        private void HandleBotSelected() {
            if (currentBot != null) {
                currentBot.OnWeaponChange -= HandleWeaponChanged;
            }

            currentBot = botSelector.CurrentBot;

            if (currentBot != null) {
                currentBot.OnWeaponChange += HandleWeaponChanged;
            }
            HandleWeaponChanged();
        }

        private void HandleWeaponChanged() {
            ClearProjectileImage();
            if (currentBot != null && currentBot.WeaponData != null) {
                if (currentBot.WeaponData == BitThrower) {
                    BitThrowerImage.SetActive(true);
                } else if (currentBot.WeaponData == HealBeam) {
                    HealBeamImage.enabled = true;
                } else if (currentBot.WeaponData == Laser) {
                    LaserImage.enabled = true;
                } else if (currentBot.WeaponData == Pulse) {
                    PulseImage.enabled = true;
                }
            }
        }

        private void ClearProjectileImage() {
            LaserImage.enabled = false;
            HealBeamImage.enabled = false;
            BitThrowerImage.SetActive(false);
            PulseImage.enabled = false;
        }
    }
}
