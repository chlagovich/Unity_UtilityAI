using Game.UtilityAI;
using UnityEngine;

namespace UtilityAI.Decisions
{
    public class SimpleDecision : IDecision
    {
        private IConsideration _consideration;
        private IAction _action;

        public SimpleDecision(string name, IConsideration consideration, IAction action)
        {
            _consideration = consideration;
            _action = action;
            Name = name;
        }

        public string Name { get; set; }

        public float Evaluate(ref Context context)
        {
            return _consideration.Evaluate(ref context);
        }

        public void OnStart(ref Context context)
        {
            _action.OnStart(ref context);
        }

        public void OnUpdate(ref Context context)
        {
            _action.OnUpdate(ref context);
        }

        public void OnFinish(ref Context context)
        {
            _action.OnFinish(ref context);
        }

        public bool IsDone(ref Context context)
        {
            return _action.IsDone(ref context);
        }
    }

    [CreateAssetMenu(menuName = "UtilityAI/Decision/Simple", order = 0)]
    public class SimpleDecisionConfig : ScriptableObject, IDecisionData
    {
        [SerializeField] private InterfaceRef<IConsiderationData> consideration;
        [SerializeField] private InterfaceRef<IActionData> action;

        public IDecision Create()
        {
            return new SimpleDecision(name, consideration.Value.Create(), action.Value.Create());
        }
    }
}