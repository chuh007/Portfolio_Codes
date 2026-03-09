using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CUH.Code.GameEvents
{
    public struct TextEvent : IEvent
    {
        public string Text;

        public TextEvent(string text)
        {
            Text = text;
        }
    }
}