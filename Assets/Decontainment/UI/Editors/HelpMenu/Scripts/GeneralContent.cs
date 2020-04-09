using System;
using UnityEngine;

namespace Editor.Help
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GeneralContent", order = 1)]
    public class GeneralContent : ScriptableObject
    {
        [Serializable]
        public struct Block
        {
            public enum Type
            {
                HEADER,
                TEXT,
                IMAGE,
                SMALL_SPACER,
                BIG_SPACER,
            }
            public Type type;
            [TextArea]
            public string text;
            public Sprite image;
            public Vector2 imageSize;
        }

        public Block[] blocks = null;
    }
}