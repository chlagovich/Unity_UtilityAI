using System;
using System.Collections.Generic;
using Game.UtilityAI;
using UnityEditor;
using UnityEngine;

namespace UtilityAI.Editor
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor : UnityEditor.Editor
    {
        private Brain _brain;
        private GUIStyle _boldStyle;
        private GUIStyle _coloredStyle;

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;
            
            _brain = (Brain)target;
            _boldStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };
            _coloredStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState { textColor = Color.green }
            };
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying && IsSceneObject(_brain.gameObject))
            {
                EditorGUILayout.Space();
                DrawBestDecision();
                EditorGUILayout.Space();
                DrawAllDecisions();
                EditorGUILayout.Space();
                DrawMemorySnapshot();
            }
        }

        private bool IsSceneObject(GameObject obj)
        {
            // Check if the object is part of a scene
            return obj != null && obj.scene.IsValid() && !string.IsNullOrEmpty(obj.scene.name);
        }
        private void DrawBestDecision()
        {
            if (_brain.GetCurrentBest() != null)
            {
                EditorGUILayout.LabelField("Best Decision:", _brain.GetCurrentBest().Name, _coloredStyle);
            }
            else
            {
                EditorGUILayout.LabelField("Best Decision:", "None", _coloredStyle);
            }
        }

        private void DrawAllDecisions()
        {
            var decisions = _brain.GetAllDecisions();

            if (decisions != null)
            {
                EditorGUILayout.LabelField("Decisions / Evaluations:", _boldStyle);

                foreach (var decision in decisions)
                {
                    EditorGUILayout.LabelField($"{decision.Item1} : {decision.Item2}");
                }
            }
        }

        private void DrawMemorySnapshot()
        {
            Dictionary<string, object> memorySnapshot = _brain.GetMemorySnapshot();

            if (memorySnapshot.Count > 0)
            {
                EditorGUILayout.LabelField("Memory Snapshot:", _boldStyle);
                foreach (var entry in memorySnapshot)
                {
                    EditorGUILayout.LabelField($"{entry.Key} : {entry.Value}");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Memory Snapshot is empty.");
            }
        }
    }
}