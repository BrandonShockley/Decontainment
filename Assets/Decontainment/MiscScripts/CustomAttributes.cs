using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PairAttribute : PropertyAttribute
{
    public string keyFieldName;
    public string valueFieldName;

    public PairAttribute(string keyFieldName, string valueFieldName)
    {
        this.keyFieldName = keyFieldName;
        this.valueFieldName = valueFieldName;
    }

    [CustomPropertyDrawer(typeof(PairAttribute))]
    private class EnumColorMapDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            PairAttribute pairAttribute = (PairAttribute)attribute;

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(
                new Rect(position.x, position.y, position.width / 2, position.height),
                property.FindPropertyRelative(pairAttribute.keyFieldName),
                GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height),
                property.FindPropertyRelative(pairAttribute.valueFieldName),
                GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
