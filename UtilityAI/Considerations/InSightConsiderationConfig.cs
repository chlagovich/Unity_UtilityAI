using Game.UtilityAI;
using UnityEngine;

namespace UtilityAI.Considerations
{
    public class InSightConsideration : IConsideration
    {
        private readonly string _tag;
        private readonly AnimationCurve _curve;

        public InSightConsideration(string tag, AnimationCurve curve)
        {
            _tag = tag;
            _curve = curve;
        }

        public float Evaluate(ref Context context)
        {
            var player = context.Vision.HasThisTag(_tag);
            return Mathf.Clamp01(player ? _curve.Evaluate(1) : _curve.Evaluate(0));
        }
    }
    
    [CreateAssetMenu(menuName = "UtilityAI/Considerations/PlayerInsight", order = 0)]
    public class InSightConsiderationConfig : ScriptableObject, IConsiderationData
    {
        [SerializeField] [GameTag] private string tag;
        [SerializeField] private AnimationCurve curve;

        public void Reset()
        {
            curve = new AnimationCurve(
                new Keyframe[3]
                {
                    new Keyframe(0, 0, 0, 0, 0, 0),
                    new Keyframe(0.5f, 1, 0, 0, 0, 0),
                    new Keyframe(1, 1, 0, 0, 0, 0)
                });
        }

        public IConsideration Create()
        {
            return new InSightConsideration(tag, curve);
        }
    }
}