using UnityEngine;

namespace Bot
{
    public class Hardpoint : MonoBehaviour
    {
        private SpriteRenderer sr;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void Init(WeaponData weaponData)
        {
            if (weaponData == null) {
                sr.enabled = false;
            } else {
                sr.enabled = true;
                sr.color = weaponData.hardpointColor;
            }
        }
    }
}
