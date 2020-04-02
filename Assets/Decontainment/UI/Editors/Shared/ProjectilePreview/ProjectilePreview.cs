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
        private Image laserImage = null;
        [SerializeField]
        private WeaponData laser = null;
        [SerializeField]
        private Image healBeamImage = null;
        [SerializeField]
        private WeaponData healBeam = null;
        [SerializeField]
        private Image pulseImage = null;
        [SerializeField]
        private WeaponData pulse = null;
        [SerializeField]
        private GameObject bitThrowerImage = null;
        [SerializeField]
        private WeaponData bitThrower = null;

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
                if (currentBot.WeaponData == bitThrower) {
                    bitThrowerImage.SetActive(true);
                } else if (currentBot.WeaponData == healBeam) {
                    healBeamImage.enabled = true;
                } else if (currentBot.WeaponData == laser) {
                    laserImage.enabled = true;
                } else if (currentBot.WeaponData == pulse) {
                    pulseImage.enabled = true;
                }
            }
        }

        private void ClearProjectileImage() {
            laserImage.enabled = false;
            healBeamImage.enabled = false;
            bitThrowerImage.SetActive(false);
            pulseImage.enabled = false;
        }
    }
}
