using System;
using UnityEngine;

namespace Game.UtilityAI
{
    public class WorldStateEntry : PropertyAttribute
    {
        public Type ExpectedType { get; }
        public WorldStateEntry(Type expectedType)
        {
            ExpectedType = expectedType;
        }
    }
}