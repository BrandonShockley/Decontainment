using UnityEngine;

namespace Bot
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public GameObject projectilePrefab;
        /// In seconds
        public float cooldown;
        public Color hardpointColor = Color.white;
        public int numShots = 1;
        ///In Degrees
        public float shotSpacing = 15f;
    }
}