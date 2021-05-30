﻿using UnityEngine;

public class ReadOnlyInspectorAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyInspectorAttribute))]
public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
{
    public override float GetPropertyHeight(UnityEditor.SerializedProperty property,
        GUIContent label)
    {
        return UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
        UnityEditor.SerializedProperty property,
        GUIContent label)
    {
        UnityEditor.EditorGUI.BeginDisabledGroup(true);
        UnityEditor.EditorGUI.PropertyField(position, property, label, true);
        UnityEditor.EditorGUI.EndDisabledGroup();
    }
}
#endif