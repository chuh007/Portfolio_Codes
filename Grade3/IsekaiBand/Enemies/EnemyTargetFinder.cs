using System.Collections.Generic;
using _Work.CHUH.Code.Item;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies
{
    internal static class EnemyTargetFinder
    {
        public static Transform FindNearest(IReadOnlyList<Enemy> enemies, IReadOnlyList<ItemChest> itemChests, Vector3 from, float range)
        {
            Transform nearest = null;
            float minDist = range * range;

            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                if (!IsValidTarget(enemy)) continue;
                TrySetNearest(enemy.transform, from, ref nearest, ref minDist);
            }

            for (int i = 0; i < itemChests.Count; i++)
            {
                ItemChest itemChest = itemChests[i];
                if (!IsValidTarget(itemChest)) continue;
                TrySetNearest(itemChest.transform, from, ref nearest, ref minDist);
            }

            return nearest;
        }

        public static Transform[] FindAllInRange(
            IReadOnlyList<Enemy> enemies,
            IReadOnlyList<ItemChest> itemChests,
            Vector3 from,
            float range)
        {
            using (ListPool<Transform>.Get(out List<Transform> result))
            {
                AddTargetsInRange(enemies, itemChests, from, range, result);
                return result.ToArray();
            }
        }

        public static Transform FindRandom(IReadOnlyList<Enemy> enemies, IReadOnlyList<ItemChest> itemChests, Vector3 from, float range)
        {
            using (ListPool<Transform>.Get(out List<Transform> inRange))
            {
                AddTargetsInRange(enemies, itemChests, from, range, inRange);
                if (inRange.Count == 0) return null;
                return inRange[Random.Range(0, inRange.Count)];
            }
        }

        internal static void AddTargetsInRange(
            IReadOnlyList<Enemy> enemies,
            IReadOnlyList<ItemChest> itemChests,
            Vector3 from,
            float range,
            List<Transform> result)
        {
            float sqrRange = range * range;
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                if (!IsValidTarget(enemy)) continue;
                Transform target = enemy.transform;
                if (IsInRange(target, from, sqrRange))
                    result.Add(target);
            }

            for (int i = 0; i < itemChests.Count; i++)
            {
                ItemChest itemChest = itemChests[i];
                if (!IsValidTarget(itemChest)) continue;
                Transform target = itemChest.transform;
                if (IsInRange(target, from, sqrRange))
                    result.Add(target);
            }
        }

        private static void TrySetNearest(Transform target, Vector3 from, ref Transform nearest, ref float minDist)
        {
            float dist = (target.position - from).sqrMagnitude;
            if (dist > minDist) return;

            minDist = dist;
            nearest = target;
        }

        private static bool IsInRange(Transform target, Vector3 from, float sqrRange)
        {
            return (target.position - from).sqrMagnitude <= sqrRange;
        }

        private static bool IsValidTarget(Enemy enemy)
        {
            return enemy != null && !enemy.IsDead && enemy.gameObject.activeInHierarchy;
        }

        private static bool IsValidTarget(ItemChest itemChest)
        {
            return itemChest != null && !itemChest.IsOpened && itemChest.gameObject.activeInHierarchy;
        }
    }
}
