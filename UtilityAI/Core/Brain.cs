using System.Collections.Generic;
using UnityEngine;

namespace Game.UtilityAI
{
    public class Brain : MonoBehaviour
    {
        [SerializeField] private InterfaceList<IDecisionData> decisionData;

        private Context _context;

        private IDecision _currentDecision = null;
        private IDecision _bestDecision = null;

        private IDecision[] _decisions;
        
        private void OnValidate()
        {
            decisionData.SetDirty();
        }

        public void Initialize(AreaType areaType)
        {
            _context = new Context(gameObject, areaType);
            _decisions = new IDecision[decisionData.Values.Count];
            for (int i = 0; i < decisionData.Values.Count; i++)
            {
                _decisions[i] = decisionData.Values[i].Create();
            }
        }

        public void Execute()
        {
            float highestUtility = float.MinValue;
            _bestDecision = null;
            for (int i = 0; i < _decisions.Length; i++)
            {
                float utility = _decisions[i].Evaluate(ref _context);
                if (utility > highestUtility)
                {
                    highestUtility = utility;
                    _bestDecision = _decisions[i];
                }
            }
        }

        private void Update()
        {
            if (_bestDecision != _currentDecision && _bestDecision != null)
            {
                if (_currentDecision != null) _currentDecision.OnFinish(ref _context);
                _currentDecision = _bestDecision;
                _currentDecision.OnStart(ref _context);
            }

            _currentDecision?.OnUpdate(ref _context);
        }
        
        public void SetMemory(string key, object value)
        {
            _context.SetMemory(key, value);
        }

        public T GetMemory<T>(string key)
        {
            return _context.GetMemory<T>(key);
        }
        
        public Dictionary<string,object> GetMemorySnapshot()
        {
            return _context.GetMemorySnapshot();
        }

        public IDecision GetCurrentBest()
        {
            return _currentDecision;
        }

        public (string, float)[] GetAllDecisions()
        {
            (string, float)[] all = new (string, float)[_decisions.Length];
            for (int i = 0; i < _decisions.Length; i++)
            {
                all[i] = new(_decisions[i].Name, _decisions[i].Evaluate(ref _context));
            }

            return all;
        }
    }
}