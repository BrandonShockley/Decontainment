﻿using Asm;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Code
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ArgTokenColorMap", order = 1)]
    public class ArgTokenColorMap : ScriptableObject
    {
        public enum Type
        {
            LOCAL_REGISTER,
            SHARED_REGISTER,
            BRANCH_LABEL,
            CONST_LABEL,
            _SIZE,
        }

        [Serializable]
        private class Entry
        {
            public Color color = Color.white;
            public Type argType = default;
        }

        public Dictionary<Type, Color> map = new Dictionary<Type, Color>();

        [SerializeField]
        [Pair("argType", "color")]
        private Entry[] entries = new Entry[(int)Type._SIZE];

        void OnEnable()
        {
            foreach (Entry e in entries) {
                if (!map.ContainsKey(e.argType)) {
                    map.Add(e.argType, e.color);
                }
            }
        }
    }
}