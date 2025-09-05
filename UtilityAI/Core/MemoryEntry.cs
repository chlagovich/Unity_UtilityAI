using System;
using UnityEngine;

namespace Game.UtilityAI
{
    public class MemoryEntry : PropertyAttribute
    {
        public Type ExpectedType { get; }
        public MemoryEntry(Type expectedType)
        {
            ExpectedType = expectedType;
        }
    }
}