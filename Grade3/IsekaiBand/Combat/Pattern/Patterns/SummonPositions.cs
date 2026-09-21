using System.Collections.Generic;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SummonPositions
    {
        private readonly SummonEnemiesPattern _pattern;

        public SummonPositions(SummonEnemiesPattern pattern) => _pattern = pattern;

        public List<SummonInfo> CreateSummonInfos(Vector2 center)
        {
            var summons = new List<SummonInfo>(_pattern.SummonCount);

            for (int i = 0; i < _pattern.SummonCount; i++)
            {
                PoolItemSO enemyItem = PickEnemyItem();
                if (enemyItem == null) return summons;

                summons.Add(new SummonInfo(enemyItem, RandomSpawnPosition(center)));
            }

            return summons;
        }

        public PoolItemSO PickEnemyItem()
        {
            for (int i = 0; i < _pattern.EnemyItems.Count; i++)
            {
                PoolItemSO item = _pattern.EnemyItems[Random.Range(0, _pattern.EnemyItems.Count)];
                if (item != null)
                    return item;
            }

            return null;
        }

        public Vector2 RandomSpawnPosition(Vector2 center)
        {
            float minDistance = Mathf.Min(_pattern.MinSpawnDistance, _pattern.MaxSpawnDistance);
            float maxDistance = Mathf.Max(_pattern.MinSpawnDistance, _pattern.MaxSpawnDistance);

            for (int i = 0; i < _pattern.PositionTryCount; i++)
            {
                Vector2 position = center + RandomDirection() * Random.Range(minDistance, maxDistance);
                if (IsInMapBounds(position))
                    return position;
            }

            Vector2 fallback = center + RandomDirection() * minDistance;
            return ClampToMapBounds(fallback);
        }

        public static Vector2 RandomDirection()
        {
            Vector2 direction = Random.insideUnitCircle;
            if (direction.sqrMagnitude <= 0.0001f)
                return Vector2.right;

            return direction.normalized;
        }

        public bool IsInMapBounds(Vector2 position)
        {
            StageHelper helper = StageHelper.Instance;
            if (helper == null)
                return true;

            return helper.CheckMapBound(position, _pattern.MapPadding);
        }

        public Vector2 ClampToMapBounds(Vector2 position)
        {
            StageHelper helper = StageHelper.Instance;
            if (helper == null)
                return position;

            return helper.ClampToMapBound(position, _pattern.MapPadding);
        }
    }
}
