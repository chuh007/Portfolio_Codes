using System.Threading;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(
        fileName = "PianoBossFairySummonPattern",
        menuName = "SO/Pattern/PianoBoss/Fairy Summon",
        order = 2)]
    public sealed class PianoBossFairySummonPatternSO : BasePatternSO
    {
        [Header("Fairy Summon")]
        [SerializeField] private PoolItemSO fairyMinionItem;
        [SerializeField, Min(1)] private int summonCount = 3;
        [SerializeField, Min(1)] private int maxLivingMinions = 6;
        [SerializeField, Min(0.5f)] private float spawnRadius = 3.2f;
        [SerializeField, Min(0.1f)] private float spawnWarningRadius = 0.65f;
        [SerializeField, Min(0f)] private float summonTelegraphDuration = 0.75f;
        [SerializeField, Min(0f)] private float summonInterval = 0.12f;
        [SerializeField, Min(0f)] private float mapPadding = 1f;
        [SerializeField] private PoolItemSO spawnVFXItem;

        private FairySummonSequence _sequence;
        private FairySummonMinions _minions;
        internal FairySummonSequence Sequence => _sequence ??= new FairySummonSequence(this);
        internal FairySummonMinions Minions => _minions ??= new FairySummonMinions(this);
        internal PoolItemSO FairyMinionItem => fairyMinionItem;
        internal int SummonCount => summonCount;
        internal int MaxLivingMinions => maxLivingMinions;
        internal float SpawnRadius => spawnRadius;
        internal float SpawnWarningRadius => spawnWarningRadius;
        internal float SummonTelegraphDuration => summonTelegraphDuration;
        internal float SummonInterval => summonInterval;
        internal float MapPadding => mapPadding;
        internal PoolItemSO SpawnVFXItem => spawnVFXItem;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Summon;
        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            owner?.GetCompo<EntityMover>()?.StopImmediately();
            await base.OnPreparePattern(owner, ct);
        }
        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct) => Sequence.OnExecutePattern(owner, ct);
        internal void PlaySound() => PlayAttackSound();
        internal void PlayVFX(PoolItemSO item, Vector3 position, float scale, Vector2 direction)
            => PlayPooledVFX(item, position, scale, direction);
    }
}
