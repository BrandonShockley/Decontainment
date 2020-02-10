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

                if (weapon.numShots > 1)
                {
                    float offset = Mathf.Floor(((float)weapon.numShots) / 2.0f - weapon.numShots);

                    for (int i = 0; i < weapon.numShots; i++)
                    {
                        Projectile.CreateProjectile(this, weapon.projectilePrefab, hardpoint.transform.position, hardpoint.transform.right + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (weapon.shotSpacing * (offset + i))), Mathf.Sin(Mathf.Deg2Rad * (weapon.shotSpacing * (offset + i))), 0));
                    }
                }
                else
                {
                    Projectile.CreateProjectile(this, weapon.projectilePrefab, hardpoint.transform.position, hardpoint.transform.right);
                }
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