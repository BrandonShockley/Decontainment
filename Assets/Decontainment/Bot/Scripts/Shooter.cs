using System;
using UnityEngine;

namespace Bot
{
    public class Shooter : MonoBehaviour
    {
        public Trigger shotRequested;
        public bool async;

        public WeaponData weaponData;

        [SerializeField]
        private Hardpoint hardpoint = null;

        private float cooldownTimer;

        public bool Running { get { return !async && cooldownTimer > 0; } }

        void FixedUpdate()
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (weaponData != null && shotRequested.Value && cooldownTimer <= 0) {
                cooldownTimer = weaponData.cooldown;
                async = true;

                Projectile.CreateProjectile(this, weaponData.projectilePrefab, hardpoint.transform.position, hardpoint.transform.right);
            }
        }

        public void Init(WeaponData weaponData)
        {
            this.weaponData = weaponData;

            hardpoint.Init(weaponData);
        }
    }
}