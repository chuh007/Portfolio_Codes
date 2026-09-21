using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.Core.Events
{
    public struct EnemyDeadEvent : IEvent
    {
        public Enemies.Enemy Enemy;

        public EnemyDeadEvent(Enemies.Enemy enemy)
        {
            Enemy = enemy;
        }
    }
}
