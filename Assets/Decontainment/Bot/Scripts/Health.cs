using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bot
{
    public class Health : MonoBehaviour
    {
        public bool vulnerable = true;

        [SerializeField]
        private int maxAmount = 5;
        [SerializeField]
        private float invulnerabilityDuration = 1;
        [SerializeField]
        private float flashInterval = 0.1f;
        [SerializeField]
        private GameObject healthBarPrefab = null;
        [SerializeField]
        private float disabledTint = 0.1f;
        [SerializeField]
        private AudioClip hitClip = null;

        private int _amount;
        /// Invulnerability timer
        private float iTimer;

        private Collider2D col;
        private GameObject healthBarGO;
        private SoundModulator sm;
        private SpriteRenderer sr;

        public bool Disabled { get { return _amount == 0; } }
        public int Amount
        {
            get { return _amount; }
            set {
                int oldHealth = _amount;
                _amount = Mathf.Min(maxAmount, Mathf.Max(value, 0));
                OnHealthChange?.Invoke();

                if (_amount == 0 && oldHealth > 0) {
                    HandleDisable();
                    OnDisable?.Invoke();
                }
            }
        }
        public int MaxAmount { get { return maxAmount; } }

        public event Action OnHealthChange;
        public event Action OnDisable;

        void Awake()
        {
            col = GetComponent<Collider2D>();
            sm = GetComponent<SoundModulator>();
            sr = GetComponent<SpriteRenderer>();

            healthBarGO = Instantiate(healthBarPrefab, GameObject.FindGameObjectWithTag("MainCanvas").transform, true);
            healthBarGO.transform.SetAsFirstSibling();
            healthBarGO.GetComponent<HealthBar>().Init(this);
        }

        void Start()
        {
            Amount = maxAmount;
        }

        void FixedUpdate()
        {
            iTimer -= Time.fixedDeltaTime;
        }

        void OnDestroy()
        {
            Destroy(healthBarGO);
        }

        public void TakeDamage(int damage)
        {
            if (vulnerable && !Disabled) {
                sm.PlayClip(hitClip);
                Amount -= damage;
                if (!Disabled) {
                    StartCoroutine(FlashRoutine());
                }
            }
        }

        public void HealUp(int healthRegain) {
            if (!Disabled) {
                Amount += healthRegain;
            }
        }

        private void HandleDisable()
        {
            Color srColor = sr.color;
            srColor.r = Mathf.Max(srColor.r - disabledTint, 0);
            srColor.g = Mathf.Max(srColor.g - disabledTint, 0);
            srColor.b = Mathf.Max(srColor.b - disabledTint, 0);
            sr.color = srColor;
            col.enabled = false;
        }

        private IEnumerator FlashRoutine()
        {
            iTimer = invulnerabilityDuration;
            while (iTimer > 0)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(flashInterval);
            }
            sr.enabled = true;
        }
    }
}