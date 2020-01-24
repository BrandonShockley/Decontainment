using System;
using UnityEngine;

namespace Bot
{
    public class Shooter : MonoBehaviour
    {
        public Trigger shotRequested;
        public bool async;

        [SerializeField]
        private float cooldown = 1;
        [SerializeField]
        private GameObject projectilePrefab = null;
        [SerializeField]
        private AudioClip projectileSound = null;

        private float cooldownTimer;

        private SoundModulator sm;

        public bool Running { get { return !async && cooldownTimer > 0; } }

        void Awake()
        {
            sm = GetComponent<SoundModulator>();
        }

        void FixedUpdate()
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (cooldownTimer <= 0 && shotRequested.Value) {
                cooldownTimer = cooldown;

                Projectile.CreateProjectile(this, projectilePrefab, transform.position, transform.right);
                sm.PlayModClip(projectileSound);
            }
        }
    }
}