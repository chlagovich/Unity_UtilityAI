using System;
using Game.UtilityAI;
using UnityEditor;
using UnityEngine;

namespace UtilityAI.Editor
{
    [CustomPropertyDrawer(typeof(DataCollectionEntry))]
    public class DataCollectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty nameProperty = property.FindPropertyRelative("name");
            SerializedProperty typeNameProperty = property.FindPropertyRelative("typeName");

            float halfWidth = position.width / 2;

            // Draw name field
            Rect nameRect = new Rect(position.x, position.y, halfWidth, position.height);
            EditorGUI.PropertyField(nameRect, nameProperty, GUIContent.none);

            // Draw type dropdown
            Rect typeRect = new Rect(position.x + halfWidth + 5, position.y, halfWidth - 5, position.height);

            int currentIndex = Array.IndexOf(DataCollection.TypeOptions, typeNameProperty.stringValue);
            if (currentIndex < 0) currentIndex = 0;

            // Draw the popup
            int selectedIndex = EditorGUI.Popup(typeRect, currentIndex, DataCollection.TypeOptions);

            typeNameProperty.stringValue = DataCollection.TypeOptions[selectedIndex];

            EditorGUI.EndProperty();
        }
    }
}