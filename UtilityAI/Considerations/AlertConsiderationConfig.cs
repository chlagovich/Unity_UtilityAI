using Game.UtilityAI;
using UnityEngine;

namespace UtilityAI.Considerations
{
    public class AlertConsideration : IConsideration
    {
        private readonly string _alertType;
        private readonly AnimationCurve _curve;

        public AlertConsideration(string alertType, AnimationCurve curve)
        {
            _alertType = alertType;
            _curve = curve;
        }

        public float Evaluate(ref Context context)
        {
            var alert = context.GetMemory<AlertType>(_alertType);
            var f = _curve.Evaluate((int)alert);

            return Mathf.Clamp01(f);
        }
    }

    [CreateAssetMenu(menuName = "UtilityAI/Considerations/Alert", order = 0)]
    public class AlertConsiderationConfig : ScriptableObject, IConsiderationData
    {
        [SerializeField] [MemoryEntry(typeof(AlertType))]
        private string alertType;

        [SerializeField] private AnimationCurve curve;

        public void Reset()
        {
            curve = new AnimationCurve(
                new Keyframe[4]
                {
                    new Keyframe(0, 0),
                    new Keyframe(1, .25f),
                    new Keyframe(2, .5f),
                    new Keyframe(3, 1)
                });
        }

        public IConsideration Create()
        {
            return new AlertConsideration(alertType, curve);
        }
    }
}