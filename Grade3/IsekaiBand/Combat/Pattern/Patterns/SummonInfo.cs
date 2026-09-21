using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal readonly struct SummonInfo
    {
            public readonly PoolItemSO EnemyItem;
            public readonly Vector2 Position;

            public SummonInfo(PoolItemSO enemyItem, Vector2 position)
            {
                EnemyItem = enemyItem;
                Position = position;
            }

    }
}
