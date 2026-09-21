using System.Collections.Generic;
using _Work.CHUH.Code.Item;
using Chuh007Lib.ObjectPool.RunTime;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Enemies
{
    internal struct SpawnerSettings
    {
        public float minDistance;
        public float maxDistance;
        public float forwardRespawnAngle;
        public float spawnViewportMargin;
        public float despawnViewportMargin;
        public float overallEnemyHealthMultiplier;
        public float rangeEnemySpawnInterval;
        public float bossCleanupFrameBudgetMilliseconds;
        public int bossCleanupMaxEnemiesPerFrame;
        public PoolManagerSO poolManager;
        public PoolItemSO itemChestPoolItem;
        public float minItemChestSpawnInterval;
        public float maxItemChestSpawnInterval;
        public float itemChestSpawnRadius;
        public float itemChestMapPadding;
        public int maxItemChestCount;
        public float itemChestRespawnDistance;
        public float minItemChestRespawnRadius;
        public float maxItemChestRespawnRadius;
        public List<WeightedPoolItem> itemChestDropTable;
    }
}
