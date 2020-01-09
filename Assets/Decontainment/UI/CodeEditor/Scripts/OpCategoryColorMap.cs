using Asm;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OpCategoryColorMap", order = 1)]
    public class OpCategoryColorMap : ScriptableObject
    {
        [Serializable]
        private class Entry
        {
            public Color color = Color.white;
            public OpCategory opCategory = default;
        }

        public Dictionary<OpCategory, Color> map = new Dictionary<OpCategory, Color>();

        [SerializeField]
        [Pair("opCategory", "color")]
        private Entry[] entries = new Entry[(int)OpCategory._SIZE];

        void OnEnable()
        {
            foreach (Entry e in entries) {
                map.Add(e.opCategory, e.color);
            }
        }
    }
}