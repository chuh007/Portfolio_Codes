using System.Collections.Generic;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class FairySummonMinions
    {
        private readonly PianoBossFairySummonPatternSO _pattern;

        public FairySummonMinions(PianoBossFairySummonPatternSO pattern) => _pattern = pattern;

        public int CountLivingMinions()
        {
            Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            int count = 0;

            for (int i = 0; i < enemies.Length; i++)
            {
                Enemy enemy = enemies[i];
                if (enemy != null
                    && !enemy.IsDead
                    && enemy.gameObject.activeInHierarchy
                    && enemy.PoolItem == _pattern.FairyMinionItem)
                {
                    count++;
                }
            }

            return count;
        }

        public List<Vector2> BuildSpawnPositions(Enemy owner, int count)
        {
            var positions = new List<Vector2>(count);
            float angleOffset = Random.Range(0f, 360f);
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = (angleOffset + angleStep * i) * Mathf.Deg2Rad;
                Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
                positions.Add(ClampToMapBounds((Vector2)owner.transform.position + direction * _pattern.SpawnRadius));
            }

            return positions;
        }

        public Enemy SpawnByPool(Enemy owner, Vector2 position)
        {
            if (_pattern.poolManager == null)
                return null;

            IPoolable poolable = _pattern.poolManager.Pop(_pattern.FairyMinionItem);
            if (poolable is not Enemy minion)
            {
                if (poolable != null)
                    _pattern.poolManager.Push(poolable);
                return null;
            }

            minion.transform.position = position;
            minion.SetTarget(owner.target);
            return minion;
        }

        public Vector2 ClampToMapBounds(Vector2 position)
        {
            StageHelper helper = StageHelper.Instance;
            return helper != null ? helper.ClampToMapBound(position, _pattern.MapPadding) : position;
        }
    }
}
