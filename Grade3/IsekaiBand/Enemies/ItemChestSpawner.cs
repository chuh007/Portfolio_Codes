using System.Collections.Generic;
using _Work.CHUH.Code.Item;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Random = UnityEngine.Random;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Enemies
{
    internal static class ItemChestSpawner
    {
        public static float TickTimer(
            float currentTimer,
            float deltaTime,
            PoolManagerSO poolManager,
            PoolItemSO itemChestPoolItem,
            List<ItemChest> activeChests,
            List<WeightedPoolItem> dropTable,
            SpawnerPositionContext positionContext,
            float spawnRadius,
            float mapPadding,
            int maxActiveChests,
            float respawnDistance,
            float minRespawnRadius,
            float maxRespawnRadius,
            float minInterval,
            float maxInterval)
        {
            ItemChestPopulation.CleanupInactiveChests(activeChests);
            ItemChestPopulation.RespawnFarChests(
                activeChests,
                positionContext,
                mapPadding,
                respawnDistance,
                minRespawnRadius,
                maxRespawnRadius);

            currentTimer -= deltaTime;
            if (currentTimer > 0f)
                return currentTimer;

            SpawnRandom(
                poolManager,
                itemChestPoolItem,
                activeChests,
                dropTable,
                positionContext,
                spawnRadius,
                mapPadding,
                maxActiveChests);

            return NextSpawnInterval(minInterval, maxInterval);
        }

        public static void SpawnRandom(
            PoolManagerSO poolManager,
            PoolItemSO itemChestPoolItem,
            List<ItemChest> activeChests,
            List<WeightedPoolItem> dropTable,
            SpawnerPositionContext positionContext,
            float spawnRadius,
            float mapPadding,
            int maxActiveChests)
        {
            if (poolManager == null || itemChestPoolItem == null) return;
            if (!positionContext.IsInfiniteMap && !positionContext.HasMapBounds) return;
            if (activeChests != null && activeChests.Count >= maxActiveChests) return;

            IPoolable poolable = poolManager.Pop(itemChestPoolItem);
            if (poolable is not ItemChest itemChest)
            {
                Debug.LogWarning($"[Spawner] {itemChestPoolItem.name} prefab needs ItemChest component.");
                if (poolable != null)
                    poolManager.Push(poolable);
                return;
            }

            activeChests.Remove(itemChest);
            activeChests.Add(itemChest);

            itemChest.transform.position = SpawnerPositionUtility.RandomItemChestSpawnPosition(
                positionContext,
                spawnRadius,
                mapPadding);
            itemChest.Initialize(poolManager, dropTable, openedChest => activeChests.Remove(openedChest));
        }

        public static float NextSpawnInterval(float minInterval, float maxInterval)
        {
            float min = Mathf.Min(minInterval, maxInterval);
            float max = Mathf.Max(minInterval, maxInterval);
            return Random.Range(min, max);
        }
    }
}
