using System;
using UnityEngine;

namespace Bot
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShooterConfigurations", order = 1)]
    public class ShooterConfigurations : ScriptableObject
    {
        [Serializable]
        public struct Configuration
        {
            public int hardpointNum;
            public Weapon weapon;
        }

        [SerializeField]
        private Configuration[] configs = null;

        public int Length { get { return configs.Length; } }

        public ShooterConfigurations Clone()
        {
            ShooterConfigurations newConfigs = ScriptableObject.CreateInstance<ShooterConfigurations>();
            newConfigs.configs = new Configuration[this.configs.Length];
            for (int i = 0; i < newConfigs.configs.Length; ++i) {
                newConfigs.configs[i] = this.configs[i];
            }
            return newConfigs;
        }

        public Configuration this[int i] { get { return configs[i]; } }
    }
}