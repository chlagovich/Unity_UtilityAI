using Game;
using Game.UtilityAI;
using UnityEngine;

namespace UtilityAI.Actions
{
    public class SearchEnemyAction : IAction
    {
        private readonly string _lastKnownPlayerLocation;
        private readonly string _lastKnownPlayerDirection;
        private readonly string _alertType;

        private readonly float _searchDuration;

        private float _lastTimePlayerLocation;
        private SpeedMode _agentSpeed;
        public SearchEnemyAction(string lastKnownPlayerLocation, string alertType, float searchDuration,
            SpeedMode agentSpeed, string lastKnownPlayerDirection)
        {
            _lastKnownPlayerLocation = lastKnownPlayerLocation;
            _alertType = alertType;
            _searchDuration = searchDuration;
            _lastTimePlayerLocation = 0.0f;
            _agentSpeed = agentSpeed;
            _lastKnownPlayerDirection = lastKnownPlayerDirection;
        }

        public void OnStart(ref Context context)
        {
            context.NavAgent.SetSpeed(_agentSpeed);
            _lastTimePlayerLocation = Time.time + _searchDuration;
            var pl = context.GetMemory<Vector3>(_lastKnownPlayerLocation);
            context.NavAgent.SampleAndMove(pl);
        }

        public void OnUpdate(ref Context context)
        {
            if (context.NavAgent.HasArrived())
            {
                // TODO : add possible area that might the player go in
                var pl = context.GetMemory<Vector3>(_lastKnownPlayerLocation);
                var dir = context.GetMemory<Vector3>(_lastKnownPlayerDirection);
                Vector3 rnd = pl + (dir.normalized * 50);
                rnd.y = 0;
                context.NavAgent.SampleAndMove(rnd);
            }

            if (_lastTimePlayerLocation < Time.time)
            {
                context.SetMemory(_lastKnownPlayerLocation, Vector3.zero);
                context.SetMemory(_alertType, AlertType.Chill);
            }
        }

        public void OnFinish(ref Context context)
        {
        }

        public bool IsDone(ref Context context)
        {
            return _lastTimePlayerLocation < Time.time;
        }
    }

    [CreateAssetMenu(menuName = "UtilityAI/Actions/Search Enemy", order = 0)]
    public class SearchEnemyActionConfig : ScriptableObject, IActionData
    {
        [SerializeField] [MemoryEntry(typeof(Vector3))]
        private string lastKnownPlayerLocation;

        [SerializeField] [MemoryEntry(typeof(Vector3))]
        private string lastKnownPlayerDirection;
        
        [SerializeField] [MemoryEntry(typeof(AlertType))]
        private string alertType;

        [SerializeField] private SpeedMode agentSpeed = SpeedMode.Run;
        [SerializeField] private float searchDuration = 10;

        public IAction Create()
        {
            return new SearchEnemyAction(lastKnownPlayerLocation, alertType, searchDuration, agentSpeed,lastKnownPlayerDirection);
        }
    }
}