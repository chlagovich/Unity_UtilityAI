using UnityEngine;
using System;
using Game.UtilityAI;
using UnityEditor;
using Object = UnityEngine.Object;


namespace UtilityAI.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceRef<>))]
    public class InterfaceRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty referenceProperty = property.FindPropertyRelative("reference");
            Type interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];

            //Type baseType = GetBaseTypeWithInterface(interfaceType);
            
            EditorGUI.BeginChangeCheck();
            UnityEngine.Object newValue = EditorGUI.ObjectField(
                position,
                label,
                referenceProperty.objectReferenceValue,
                typeof(ScriptableObject),
                false
            );

            if (EditorGUI.EndChangeCheck())
            {
                if (newValue == null || interfaceType.IsInstanceOfType(newValue))
                {
                    referenceProperty.objectReferenceValue = newValue;
                    property.FindPropertyRelative("guid").stringValue =
                        newValue != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newValue)) : null;
                }
                else
                {
                    Debug.LogWarning($"Selected object does not implement {interfaceType.Name}");
                }
            }

            EditorGUI.EndProperty();
        }
    }
}