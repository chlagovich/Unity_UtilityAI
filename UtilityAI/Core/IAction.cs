namespace Game.UtilityAI
{
    public interface IAction
    {
        void OnStart(ref Context context);
        void OnUpdate(ref Context context);
        void OnFinish(ref Context context);
        bool IsDone(ref Context context);
    }
}