using Code.CoreSystem;

namespace Work.CHUH._01Scripts.Event
{
    public class EnemyDeadEvent : IEvent
    {
        public int gold;

        public EnemyDeadEvent(int value)
        {
            gold = value;
        }
    }
}