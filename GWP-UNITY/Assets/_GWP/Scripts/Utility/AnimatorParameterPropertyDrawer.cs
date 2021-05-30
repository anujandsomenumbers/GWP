using System.Linq;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
public class AnimatorParameterPropertyDrawer : PropertyDrawer
{
    private const string noParameterLabel = "SELECT A PARAMETER";
    private bool didTryInitialize = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Animator animator = null;
        if (property.serializedObject.targetObject is MonoBehaviour monoBehaviour)
        {
            animator = monoBehaviour.GetComponentInChildren<Animator>();
        }
        
        if (!animator || 
            !IsAcceptableType(property.propertyType) || 
            !CheckHasParameters(animator))
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        label = EditorGUI.BeginProperty(position, label, property);

        List<string> parameters = animator.parameters.Select(p => p.name).ToList();
        parameters.Insert(0, noParameterLabel);
        int currentIndex = property.propertyType == SerializedPropertyType.String 
            ? parameters.IndexOf(property.stringValue)
            : parameters.FindIndex(name => Animator.StringToHash(name) == property.intValue);
        currentIndex = Mathf.Clamp(currentIndex, 0, parameters.Count - 1);
        currentIndex = EditorGUI.Popup(position, label, currentIndex, 
            parameters.Select(p => new GUIContent(p)).ToArray());

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            property.intValue = Animator.StringToHash(parameters[currentIndex]);
        }
        else if (property.propertyType == SerializedPropertyType.String)
        {
            property.stringValue = parameters[currentIndex];
        }
    }

    private bool CheckHasParameters(Animator animator)
    {
        // Refresh the animator to get the right parameter count.
        if (!animator.isInitialized && !didTryInitialize)
        {
            animator.enabled = !animator.enabled;
            animator.enabled = !animator.enabled;
            if (!animator.isInitialized)
            {
                didTryInitialize = true;
                return false;
            }
        }
        return 0 != animator.parameterCount;
    }

    private bool IsAcceptableType(SerializedPropertyType type) =>
        type == SerializedPropertyType.String
        || type == SerializedPropertyType.Integer;

}
#endif

[System.AttributeUsage(System.AttributeTargets.Field)]
public class AnimatorParameterAttribute : PropertyAttribute { }
