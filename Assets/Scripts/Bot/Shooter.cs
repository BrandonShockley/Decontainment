using System;
using UnityEngine;

namespace Bot
{
    public class Shooter : MonoBehaviour
    {
        public Action onComplete;

        public Trigger shotRequested;

        [SerializeField]
        private float cooldown = 1;
        [SerializeField]
        private GameObject projectilePrefab = null;
        [SerializeField]
        private AudioClip projectileSound = null;

        private float cooldownTimer;

        private SoundModulator sm;

        void Awake()
        {
            sm = GetComponent<SoundModulator>();
        }

        void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0 && shotRequested.Value) {
                cooldownTimer = cooldown;

                Projectile.CreateProjectile(this, projectilePrefab, transform.position, transform.right);
                sm.PlayModClip(projectileSound);
                onComplete?.Invoke();
            }
        }
    }
}