namespace Code.Core.EventSystems
{
    public static class UIEvents
    {
        public static FadeEvent FadeEvent = new FadeEvent();
        public static FadeCompleteEvent FadeCompleteEvent = new FadeCompleteEvent();
        public static readonly HPValueChangeEvent HPValueChangeEvent = new HPValueChangeEvent();
        public static readonly OnsetUIEvent OnsetUIEvent = new OnsetUIEvent();
        public static readonly EscEvent EscEvent = new EscEvent();
    }

    public class FadeEvent : GameEvent
    {
        public bool isFadeIn;
        public float fadeTime;
        public bool isSaveOrLoad; //저장이나 로딩을 하는거냐?
    }

    public class FadeCompleteEvent : GameEvent
    { }

    public class HPValueChangeEvent : GameEvent
    {
        public float value;
        public float maxValue;
    }

    public class OnsetUIEvent : GameEvent
    {
        public bool isON;
    }

    public class EscEvent : GameEvent
    {
        
    }
}