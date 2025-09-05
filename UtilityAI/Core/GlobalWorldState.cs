using System.Collections.Generic;
using UnityEngine;

namespace Game.UtilityAI
{
    public class GlobalWorldState : MonoBehaviour
    {
        public static GlobalWorldState Instance { get; private set; }

        private Dictionary<string, object> _worldState = new Dictionary<string, object>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void SetWorldState(string key, object value)
        {
            _worldState[key] = value;
        }

        public T GetWorldState<T>(string key)
        {
            if (_worldState.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            
            Debug.LogWarning(key + " not found in world State");
            return default;
        }
    }
}