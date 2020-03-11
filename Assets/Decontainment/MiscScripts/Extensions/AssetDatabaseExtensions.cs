#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Extensions
{
    public class AssetDatabaseExtensions
    {
        public static T[] LoadAllAssetsInDir<T>(string dirPath) where T : Object
        {
            List<T> assets = new List<T>();
            foreach (string filePath in Directory.GetFiles(dirPath)) {
                T data = AssetDatabase.LoadAssetAtPath<T>(filePath);
                if (data != null) {
                    assets.Add(data);
                }
            }
            return assets.ToArray();
        }
    }
}

#endif