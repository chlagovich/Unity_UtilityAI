using System.Collections.Generic;
using System.Linq;
using Game.UtilityAI;
using UnityEngine;

namespace UtilityAI.Decisions
{
    public enum CalculationType
    {
        Add,
        Multiply,
        Average,
        Subtract,
        Divide,
        Max,
        Min
    }

    public class SequenceDecision : IDecision
    {
        private CalculationType _calculationType;
        private IConsideration[] _considerations;
        private IAction[] _actions;

        private int _currentActionIndex = 0;

        public SequenceDecision(string name, CalculationType calculationType, IConsideration[] considerations,
            IAction[] actions)
        {
            _calculationType = calculationType;
            _considerations = considerations;
            _actions = actions;
            Name = name;
        }

        public string Name { get; set; }

        public float Evaluate(ref Context context)
        {
            if (_considerations.Length == 0)
                return 0;

            float firstConsideration = _considerations[0].Evaluate(ref context);

            if (_considerations.Length == 1)
                return firstConsideration;

            List<float> values = new List<float>();
            values.Add(firstConsideration);
            for (int i = 1; i < _considerations.Length; i++)
            {
                values.Add(_considerations[i].Evaluate(ref context));
            }

            float result = _calculationType switch
            {
                CalculationType.Add => values.Sum(),
                CalculationType.Average => values.Average(),
                CalculationType.Multiply => values.Aggregate((a, b) => a * b),
                CalculationType.Min => values.Min(),
                CalculationType.Max => values.Max(),
                CalculationType.Subtract => values.Aggregate((a, b) => a - b),
                CalculationType.Divide => values.Aggregate((a, b) => a / Mathf.Max(b, 0.01f)),
                _ => 0
            };

            return Mathf.Clamp01(result);
        }

        public void OnStart(ref Context context)
        {
            _currentActionIndex = 0;
            if (_actions.Length > 0)
                _actions[_currentActionIndex].OnStart(ref context);
        }

        public void OnUpdate(ref Context context)
        {
            if (_currentActionIndex >= _actions.Length) return;

            var currentAction = _actions[_currentActionIndex];
            currentAction.OnUpdate(ref context);

            if (currentAction.IsDone(ref context))
            {
                currentAction.OnFinish(ref context);
                _currentActionIndex++;

                if (_currentActionIndex < _actions.Length)
                    _actions[_currentActionIndex].OnStart(ref context);
            }
        }


        public void OnFinish(ref Context context)
        {
        }

        public bool IsDone(ref Context context)
        {
            return _currentActionIndex >= _actions.Length;
        }
    }

    [CreateAssetMenu(menuName = "UtilityAI/Decision/Sequence", order = 0)]
    public class SequenceDecisionConfig : ScriptableObject, IDecisionData
    {
        [SerializeField] private CalculationType calculationType;
        [SerializeField] private InterfaceList<IConsiderationData> considerations;
        [SerializeField] private InterfaceList<IActionData> actions;

        private void OnValidate()
        {
            considerations.SetDirty();
            actions.SetDirty();
        }

        public IDecision Create()
        {
            IConsideration[] cons = new IConsideration[considerations.Values.Count];
            for (int i = 0; i < considerations.Values.Count; i++)
            {
                var c = considerations.Values[i];
                cons[i] = c.Create();
            }

            IAction[] acs = new IAction[actions.Values.Count];
            for (int i = 0; i < actions.Values.Count; i++)
            {
                var c = actions.Values[i];
                acs[i] = c.Create();
            }

            return new SequenceDecision(name, calculationType, cons, acs);
        }
    }
}