using System.Collections.Generic;
using _Work.CHUH.Code.Item;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Random = UnityEngine.Random;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Enemies
{
    internal static class ItemChestPopulation
    {
        public static void CleanupInactiveChests(List<ItemChest> activeChests)
        {
            if (activeChests == null) return;

            for (int i = activeChests.Count - 1; i >= 0; i--)
            {
                ItemChest itemChest = activeChests[i];
                if (itemChest == null || itemChest.IsOpened || !itemChest.gameObject.activeInHierarchy)
                    activeChests.RemoveAt(i);
            }
        }

        public static void RespawnFarChests(
            List<ItemChest> activeChests,
            SpawnerPositionContext positionContext,
            float mapPadding,
            float respawnDistance,
            float minRespawnRadius,
            float maxRespawnRadius)
        {
            if (activeChests == null || positionContext.Player == null) return;

            float sqrRespawnDistance = respawnDistance * respawnDistance;
            for (int i = 0; i < activeChests.Count; i++)
            {
                ItemChest itemChest = activeChests[i];
                if (itemChest == null || itemChest.IsOpened || !itemChest.gameObject.activeInHierarchy)
                    continue;

                if (((Vector2)itemChest.transform.position - positionContext.PlayerPosition).sqrMagnitude < sqrRespawnDistance)
                    continue;

                itemChest.transform.position = SpawnerPositionUtility.RandomItemChestSpawnPosition(
                    positionContext,
                    minRespawnRadius,
                    maxRespawnRadius,
                    mapPadding);
            }
        }
    }
}
