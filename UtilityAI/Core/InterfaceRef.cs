using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.UtilityAI
{
    [Serializable]
    public class InterfaceRef<T> where T : class
    {
        [SerializeField] private UnityEngine.Object reference;
        [SerializeField] private string guid;

        public T Value
        {
            get
            {
#if UNITY_EDITOR
                if (reference == null && !string.IsNullOrEmpty(guid))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    reference = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                }
#endif
                return reference as T;
            }
            set
            {
                reference = value as UnityEngine.Object;
#if UNITY_EDITOR
                if (reference != null)
                {
                    string path = AssetDatabase.GetAssetPath(reference);
                    guid = AssetDatabase.AssetPathToGUID(path);
                }
                else
                {
                    guid = null;
                }
#endif
            }
        }
    }

    [Serializable]
    public class InterfaceList<T> where T : class
    {
        [SerializeField] private List<UnityEngine.Object> references = new ();

        private List<T> _cachedValues;
        private bool _isDirty = true;
        public IReadOnlyList<T> Values
        {
            get
            {
                if (_isDirty)
                {
                    _cachedValues = references.ConvertAll(r => r as T);
                    _isDirty = false;
                }
                return _cachedValues;
            }
        }

        public void SetDirty()
        {
            _isDirty = true;
        }
    }
}