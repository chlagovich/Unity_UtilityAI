using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.UtilityAI
{
    public class Context
    {
        public Transform Target { get; }
        public Sensor Vision { get; }
        public AIMove NavAgent { get; }
        public Animator Anim { get; }

        public AreaType myArea { get; }

        private readonly Dictionary<string, object> _memory = new();

        public Context(GameObject gameObject, AreaType area)
        {
            NavAgent = gameObject.GetComponent<AIMove>();
            Anim = gameObject.GetComponent<Animator>();
            Target = gameObject.transform;
            Vision = gameObject.GetComponent<Sensor>();
            myArea = area;
        }

        public void SetMemory(string key, object value)
        {
            _memory[key] = value;
        }

        public T GetMemory<T>(string key)
        {
            if (_memory.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public Dictionary<string, object> GetMemorySnapshot()
        {
            return _memory;
        }
    }
}