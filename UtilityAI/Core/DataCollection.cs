using System;
using UnityEngine;

namespace Game.UtilityAI
{
    [Serializable]
    public class DataCollectionEntry
    {
        [SerializeField] private string name;
        [SerializeField] private string typeName;

        public string Name => name;
        public string TypeName => typeName;
    }

    public abstract class DataCollection : ScriptableObject
    {
        public static readonly string[] TypeOptions = { "float", "int", "string", "bool", "Vector3", "AlertType" };
        [SerializeField] private DataCollectionEntry[] collection;
        public DataCollectionEntry[] Collection => collection;

        public static bool IsTypeMatch(string typeName, Type targetType)
        {
            // Check if the type name matches the target type
            switch (typeName)
            {
                case "int":
                    return targetType == typeof(int);
                case "float":
                    return targetType == typeof(float);
                case "string":
                    return targetType == typeof(string);
                case "bool":
                    return targetType == typeof(bool);
                case "Vector2":
                    return targetType == typeof(Vector2);
                case "Vector3":
                    return targetType == typeof(Vector3);
                case "AlertType":
                    return targetType == typeof(AlertType);
                default:
                    return false;
            }
        }
    }
}