using System;
using UnityEngine;

namespace Bot
{
    public class Shooter : MonoBehaviour
    {
        public Trigger shotRequested;
        public bool async;

        public Hardpoint hardpoint;
        public WeaponData weaponData;

        private float cooldownTimer;

        public bool Running { get { return !async && cooldownTimer > 0; } }

        void FixedUpdate()
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (shotRequested.Value && cooldownTimer <= 0) {
                cooldownTimer = weaponData.cooldown;
                async = true;

<<<<<<< HEAD
                if (weapon.numShots > 1)
                {
                    float offset = Mathf.Ceil(((float) weapon.numShots) / 2.0f - weapon.numShots);

                    for (int i = 0; i < weapon.numShots; i++)
                    {
                        Projectile.CreateProjectile(this, weapon.projectilePrefab, hardpoint.transform.position, hardpoint.transform.right + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (weapon.shotSpacing * (offset + i) + hardpoint.transform.eulerAngles.z)),
                                                                                                                                               Mathf.Sin(Mathf.Deg2Rad * (weapon.shotSpacing * (offset + i) + hardpoint.transform.eulerAngles.z)), 0));
                    }
                }
                else
                {
                    Projectile.CreateProjectile(this, weapon.projectilePrefab, hardpoint.transform.position, hardpoint.transform.right);
                }
=======
                Projectile.CreateProjectile(this, weaponData.projectilePrefab, hardpoint.transform.position, hardpoint.transform.right);
>>>>>>> 5e29002480a6f74d7cd9dc3c6367b38ec854bcbb
            }
        }

        public void Init(Hardpoint hardpoint, WeaponData weaponData)
        {
            this.hardpoint = hardpoint;
            this.weaponData = weaponData;

            this.hardpoint.Init(weaponData.hardpointColor);
        }
    }
}