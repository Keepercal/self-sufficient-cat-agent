using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HeaderGroupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HeaderGroupAttribute headerGroup = (HeaderGroupAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        // Header label
        if (property.displayName == headerGroup.header)
        {
            EditorGUI.LabelField(position, headerGroup.header, EditorStyles.boldLabel);
        }

        // Indent
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(property, label, true);
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.singleLineHeight;
    }
}
