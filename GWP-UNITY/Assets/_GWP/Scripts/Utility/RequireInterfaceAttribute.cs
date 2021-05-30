using UnityEngine;
using System;

#if UNITY_EDITOR

using UnityEditor;

[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (RequireInterfaceAttribute)this.attribute;

        label.text += $" : {attribute.NameString}";
        label.tooltip = $"{property.displayName} must have components implementing: {attribute.NameString}";
        int oldId = property.objectReferenceInstanceIDValue;

        EditorGUI.PropertyField(position, property, label);
        if (property.objectReferenceInstanceIDValue == oldId) return;

        if (!DoesImplementInterfaces(property, attribute.interfaceTypes))
        {
            property.objectReferenceValue = null;
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    private bool DoesImplementInterfaces(SerializedProperty property, Type[] interfaceTypes)
    {
        foreach (var type in interfaceTypes)
        {
            if (!DoesImplementInterface(property, type)) return false;
        }

        return true;
    }

    private bool DoesImplementInterface(SerializedProperty property, Type interfaceType)
    {
        UnityEngine.Object obj;
        if (null == (obj = property.objectReferenceValue)) return false;

        if (obj is GameObject go) return null != go.GetComponent(interfaceType);

        return obj.GetType().Equals(interfaceType);
    }
}

#endif

[AttributeUsage(AttributeTargets.Field)]
public class RequireInterfaceAttribute : PropertyAttribute
{
    public string NameString
    {
        get
        {
            string name = "";
            for (int i = 0; i < interfaceTypes.Length; i++)
            {
                string delim = 0 == i ? "" : ",";
                name += $"{ delim } {interfaceTypes[i].Name}";
            }
            return name.Trim();
        }
    }

    public Type[] interfaceTypes;

    public RequireInterfaceAttribute(params Type[] types) => interfaceTypes = types;
}