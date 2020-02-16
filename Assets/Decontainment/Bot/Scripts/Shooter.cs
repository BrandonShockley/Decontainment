using System;
using UnityEngine;

namespace Bot
{
    public class Shooter : MonoBehaviour
    {
        public Trigger shotRequested;
        public bool async;

        public Hardpoint hardpoint;
        public Weapon weapon;

        private float cooldownTimer;

        public bool Running { get { return !async && cooldownTimer > 0; } }

        void FixedUpdate()
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (shotRequested.Value && cooldownTimer <= 0) {
                cooldownTimer = weapon.cooldown;
                async = true;

                Projectile.CreateProjectile(this, weapon.projectilePrefab, hardpoint.transform.position, hardpoint.transform.right);
            }
        }

        public void Init(Hardpoint hardpoint, Weapon weapon)
        {
            this.hardpoint = hardpoint;
            this.weapon = weapon;

            this.hardpoint.Init(weapon.hardpointColor);
        }
    }
}