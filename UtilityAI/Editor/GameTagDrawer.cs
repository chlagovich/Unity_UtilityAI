using Game.UtilityAI;
using UnityEditor;
using UnityEngine;

namespace UtilityAI.Editor
{
    [CustomPropertyDrawer(typeof(GameTag))]
    public class GameTagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                // Get all available tags
                var tags = UnityEditorInternal.InternalEditorUtility.tags;

                // Find the current tag's index
                int index = System.Array.IndexOf(tags, property.stringValue);
                index = Mathf.Max(index, 0);  // Default to 0 if not found

                // Create the tag popup
                int newIndex = EditorGUI.Popup(position, label.text, index, tags);

                // Update the property value if a new tag is selected
                property.stringValue = tags[newIndex];
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}