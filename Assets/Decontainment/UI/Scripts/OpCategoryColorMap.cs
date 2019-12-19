using Asm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OpCategoryColorMap", order = 1)]
public class OpCategoryColorMap : ScriptableObject
{
    [Serializable]
    private class Entry
    {
        public OpCategory opCategory = default;
        public Color color = Color.white;
    }
    [CustomPropertyDrawer(typeof(Entry))]
    private class EntryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width / 2, position.height),
                property.FindPropertyRelative("opCategory"),
                GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height),
                property.FindPropertyRelative("color"),
                GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

    public Dictionary<OpCategory, Color> map = new Dictionary<OpCategory, Color>();

    [SerializeField]
    private Entry[] entries = new Entry[(int)OpCategory._SIZE];

    void OnEnable()
    {
        foreach (Entry e in entries) {
            map.Add(e.opCategory, e.color);
        }
    }
}
