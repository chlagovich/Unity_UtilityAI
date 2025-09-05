namespace Game.UtilityAI
{
    public interface IConsideration
    {
        float Evaluate(ref Context context); 
    }
}