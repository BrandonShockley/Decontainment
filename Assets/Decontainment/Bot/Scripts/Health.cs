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

        private GameObject healthBarGO;
        private SoundModulator sm;
        private SpriteRenderer sr;

        public bool Disabled { get { return _amount == 0; } }
        public int Amount
        {
            get { return _amount; }
            set {
                int oldHealth = _amount;
                _amount = Mathf.Max(value, 0);
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
            sm = GetComponent<SoundModulator>();
            sr = GetComponent<SpriteRenderer>();

            healthBarGO = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity,
                FindObjectOfType<Canvas>().transform);
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
                Amount -= damage;
                sm.PlayModClip(hitClip);
                if (!Disabled) {
                    StartCoroutine(InvulnerableRoutine());
                }
            }
        }

        private void HandleDisable()
        {
            Color srColor = sr.color;
            srColor.r = Mathf.Max(srColor.r - disabledTint, 0);
            srColor.g = Mathf.Max(srColor.g - disabledTint, 0);
            srColor.b = Mathf.Max(srColor.b - disabledTint, 0);
            sr.color = srColor;
        }

        private IEnumerator InvulnerableRoutine()
        {
            vulnerable = false;
            iTimer = invulnerabilityDuration;
            while (iTimer > 0)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(flashInterval);
            }
            sr.enabled = true;
            vulnerable = true;
        }
    }
}