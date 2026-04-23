namespace _01Scripts.Core.EventSystem
{
    public class PlayerEvents
    {
        public static readonly AddEXPEvent AddExpEvent = new AddEXPEvent();
        public static readonly PlayerUIChangeEvent PlayerUIChangeEvent = new PlayerUIChangeEvent();
    }
    
    public class AddEXPEvent : GameEvent
    {
        public int exp;
    }

    public class PlayerUIChangeEvent : GameEvent
    {
        public ControlUIType ControlUIType;
    }
}