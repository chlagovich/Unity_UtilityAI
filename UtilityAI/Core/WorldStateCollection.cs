#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.UtilityAI
{
    [CreateAssetMenu(fileName = "Assets/Game Presets/World State Collection",menuName = "UtilityAI/World State Collection", order = 0)]
    public class WorldStateCollection : DataCollection
    {
#if UNITY_EDITOR
        public static WorldStateCollection Load()
        {
            return AssetDatabase.LoadAssetAtPath<WorldStateCollection>("Assets/Game Presets/World State Collection.asset");
        }
#endif
    }
}