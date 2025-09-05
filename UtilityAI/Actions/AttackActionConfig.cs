using Game.UtilityAI;
using UnityEngine;

namespace UtilityAI.Actions
{
    public class AttackAction : IAction
    {
        public void OnStart(ref Context context)
        {
            context.NavAgent.CancelPath();
            context.NavAgent.SetRotation(false);
        }
        
        public void OnUpdate(ref Context context)
        {
            var player = context.Vision.GetByTag("Player");
            if(!player)
                return;
            var rot = Quaternion.LookRotation(player.transform.position - context.Target.position);
            context.Target.rotation = Quaternion.Slerp(context.Target.rotation,rot,Time.deltaTime * 10);
        }

        public void OnFinish(ref Context context)
        {
            context.NavAgent.SetRotation(true);
        }

        public bool IsDone(ref Context context)
        {
            return true;
        }
    }
    
    [CreateAssetMenu(menuName = "UtilityAI/Actions/Attack", order = 0)]
    public class AttackActionConfig : ScriptableObject, IActionData
    {

        public IAction Create()
        {
            return new AttackAction();
        }
    }
}