using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Command;

namespace Work.CUH.Code.GameEvents
{
    public struct CommandEvent : IEvent
    {
        public BaseCommand Command { get; private set; }

        public CommandEvent(BaseCommand command)
        {
            Command = command;
        }
    }
}