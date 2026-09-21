using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Core.Events
{
    public struct EnemySpawnEvent : IEvent
    {
        public PoolItemSO PoolItem;
        public int RewardCount;
        public bool UseBossDamageAttenuation;

        public EnemySpawnEvent(
            PoolItemSO poolItem,
            int rewardCount = 0,
            bool useBossDamageAttenuation = false)
        {
            PoolItem = poolItem;
            RewardCount = rewardCount;
            UseBossDamageAttenuation = useBossDamageAttenuation;
        }
    }

    public struct BossSpawnedEvent : IEvent
    {
        public Enemy Boss;

        public BossSpawnedEvent(Enemy boss)
        {
            Boss = boss;
        }
    }

    public struct BossApproachingEvent : IEvent
    {
    }

    public struct BossPreludeSpawnedEvent : IEvent
    {
        public Enemy Boss;

        public BossPreludeSpawnedEvent(Enemy boss)
        {
            Boss = boss;
        }
    }

    public struct Boss1EncounterStartedEvent : IEvent
    {
        public Enemy Boss;

        public Boss1EncounterStartedEvent(Enemy boss)
        {
            Boss = boss;
        }
    }
}
