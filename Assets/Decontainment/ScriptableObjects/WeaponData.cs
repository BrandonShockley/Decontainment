using UnityEngine;

namespace Bot
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public const string RESOURCES_DIR = "Weapons";

        public GameObject projectilePrefab;
        public bool operateInBotSpace;
        /// In seconds
        public float cooldown;
        public Color hardpointColor = Color.white;
        public int numShots = 1;
        ///In Degrees
        public float shotSpacing = 15f;

        public override string ToString() { return name; }
    }
}