using _01Scripts.Enemies;

namespace _01Scripts.Core.EventSystem
{
    public static class EnemyEvents
    {
        public static readonly EnemyActionEvent EnemyActionEvent = new EnemyActionEvent();
        public static readonly EnemyDeadEvent EnemyDeadEvent = new EnemyDeadEvent();
    }

    public class EnemyActionEvent : GameEvent
    {
        public string description;
    }
    
    public class EnemyDeadEvent : GameEvent
    {
        public Enemy Enemy;
    }
}