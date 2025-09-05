using System;
using System.Linq;
using Game.UtilityAI;
using UnityEditor;
using UnityEngine;

namespace UtilityAI.Editor
{
    [CustomPropertyDrawer(typeof(MemoryEntry))]
    public class MemoryEntryDrawer : PropertyDrawer
    {
        private AIMemoryCollection _collection;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            if (_collection == null)
            {
                _collection = AIMemoryCollection.Load();
                if (_collection == null)
                {
                    GUI.color = Color.red;
                    EditorGUI.LabelField(position, "No Memory Collection Found.");
                    return;
                }
            }

            var att = (MemoryEntry)attribute;
            // Filtering attributes based on type
            var matchingAttributes = _collection.Collection
                .Where(a => DataCollection.IsTypeMatch(a.TypeName, att.ExpectedType))
                .ToList();
        
            var attributeNames = matchingAttributes.Select(attr => attr.Name).ToArray();
            int currentIndex = Mathf.Max(0, Array.IndexOf(attributeNames, property.stringValue));
            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, attributeNames);

            // Update the selected attribute name
            if (selectedIndex >= 0 && selectedIndex < attributeNames.Length)
            {
                property.stringValue = attributeNames[selectedIndex];
            }

            EditorGUI.EndProperty();
        }
    }
}