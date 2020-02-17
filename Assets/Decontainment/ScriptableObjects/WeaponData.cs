using UnityEngine;

namespace Bot
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/Weapon", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public const string RESOURCES_DIR = "Weapons";

        public GameObject projectilePrefab;
        /// In seconds
        public float cooldown;
        public Color hardpointColor = Color.white;
    }
}