using _01Scripts.TurnSystem;

namespace _01Scripts.Core.EventSystem
{
    public class TurnEvents
    {
        public static readonly TurnStartEvent TurnStartEvent = new TurnStartEvent();
        public static readonly TurnEndEvent TurnEndEvent = new TurnEndEvent();
        public static readonly TurnUIEvent TurnUIEvent = new TurnUIEvent();
    }
    public class TurnStartEvent : GameEvent
    {
        
    }
    public class TurnEndEvent : GameEvent
    {
        
    }

    public class TurnUIEvent : GameEvent
    {
        
    }
}