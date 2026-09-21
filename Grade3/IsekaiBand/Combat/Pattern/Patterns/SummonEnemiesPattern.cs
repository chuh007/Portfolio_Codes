using System.Collections.Generic;
using System.Threading;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.ObjectPool.RunTime;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "SummonEnemiesPattern", menuName = "SO/Pattern/SummonEnemiesPattern", order = 0)]
    public class SummonEnemiesPattern : BasePatternSO
    {
        [SerializeField] private List<PoolItemSO> enemyItems = new();
        [SerializeField, Min(1)] private int summonCount = 3;
        [SerializeField, Min(0f)] private float minSpawnDistance = 1.5f;
        [SerializeField, Min(0.1f)] private float maxSpawnDistance = 6f;
        [SerializeField, Min(0f)] private float mapPadding = 1f;
        [SerializeField, Min(0.1f)] private float spawnWarningRadius = 0.75f;
        [SerializeField, Min(1)] private int positionTryCount = 10;
        [SerializeField, Min(0f)] private float summonDelay = 1f;
        [SerializeField, Min(0f)] private float summonInterval = 0f;
        [SerializeField] private PoolItemSO spawnVFXItem;

        private SummonSequence _sequence;
        private SummonPositions _positions;
        private SummonSpawn _spawn;
        internal SummonSequence Sequence => _sequence ??= new SummonSequence(this);
        internal SummonPositions Positions => _positions ??= new SummonPositions(this);
        internal SummonSpawn Spawn => _spawn ??= new SummonSpawn(this);
        internal List<PoolItemSO> EnemyItems => enemyItems;
        internal int SummonCount => summonCount;
        internal float MinSpawnDistance => minSpawnDistance;
        internal float MaxSpawnDistance => maxSpawnDistance;
        internal float MapPadding => mapPadding;
        internal float SpawnWarningRadius => spawnWarningRadius;
        internal int PositionTryCount => positionTryCount;
        internal float SummonDelay => summonDelay;
        internal float SummonInterval => summonInterval;
        internal PoolItemSO SpawnVFXItem => spawnVFXItem;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Summon;
        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        internal void PlaySound() => PlayAttackSound();
        internal void PlayVFX(PoolItemSO item, Vector3 position, float scale, Vector2 direction)
            => PlayPooledVFX(item, position, scale, direction);
    }
}
