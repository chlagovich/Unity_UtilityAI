#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.UtilityAI
{
    [CreateAssetMenu(fileName = "Assets/Game Presets/Memory Collection",menuName = "UtilityAI/Memory Collection", order = 0)]
    public class AIMemoryCollection : DataCollection
    {
        
#if UNITY_EDITOR
        public static AIMemoryCollection Load()
        {
            return AssetDatabase.LoadAssetAtPath<AIMemoryCollection>("Assets/Game Presets/Memory Collection.asset");
        }
#endif
    }
}