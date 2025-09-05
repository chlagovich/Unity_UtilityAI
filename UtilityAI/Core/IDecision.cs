namespace Game.UtilityAI
{
    public interface IDecision
    {
        public string Name { get; set; }
        float Evaluate(ref Context context); 
        void OnStart(ref Context context);
        void OnUpdate(ref Context context);
        void OnFinish(ref Context context);
        bool IsDone(ref Context context);
    }
}