using Game;
using Game.UtilityAI;
using UnityEngine;

namespace UtilityAI.Actions
{
    public class RoamAction : IAction
    {
        private float _minIdleTime;
        private float _maxIdleTime;
        private bool _hasArrived = false;
        private float _waitTime;

        public RoamAction(float minIdleTime, float maxIdleTime)
        {
            _minIdleTime = minIdleTime;
            _maxIdleTime = maxIdleTime;
        }
        public void OnStart(ref Context context)
        {
            context.NavAgent.SetSpeed(SpeedMode.Walk);
            PickDestination(ref context);
        }

        private void PickDestination(ref Context context)
        {
            Vector3 pos = GameLoop.Instance.Get<AreaSystem>().GetRandomPointInArea(context.myArea);
            context.NavAgent.SampleAndMove(pos);
            _hasArrived = false;
        }

        public void OnUpdate(ref Context context)
        {
            if (context.NavAgent.HasArrived())
            {
                if (!_hasArrived)
                {
                    _hasArrived = true;
                    _waitTime = Random.Range(_minIdleTime, _maxIdleTime) + Time.time;
                }

                if (Time.time > _waitTime)
                {
                    PickDestination(ref context);
                }
            }
        }

        public void OnFinish(ref Context context)
        {
        }

        public bool IsDone(ref Context context)
        {
            return false;
        }
    }
    
    [CreateAssetMenu(menuName = "UtilityAI/Actions/Roam", order = 0)]
    public class RoamActionConfig : ScriptableObject, IActionData
    {
        [SerializeField] private float minIdleTime = 3;
        [SerializeField] private float maxIdleTime = 6;
        
        public IAction Create()
        {
            return new RoamAction(minIdleTime, maxIdleTime);
        }
    }
}