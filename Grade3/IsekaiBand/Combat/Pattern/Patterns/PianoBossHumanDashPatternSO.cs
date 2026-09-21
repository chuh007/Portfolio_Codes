using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(
        fileName = "PianoBossHumanDashPattern",
        menuName = "SO/Pattern/PianoBoss Human/Anticipation Dash",
        order = 3)]
    public sealed class PianoBossHumanDashPatternSO : BasePatternSO
    {
        [Header("Dash")]
        [SerializeField, Min(0f)] private float dashDistance = 5f;
        [SerializeField, Min(0.01f)] private float dashDuration = 0.2f;
        [SerializeField, Min(0.1f)] private float warningWidth = 1f;
        [SerializeField, Min(0f)] private float boundaryPadding = 0.5f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.8f;
        [SerializeField] private ContactFilter2D whatIsTarget;
        [SerializeField] private Chuh007Lib.ObjectPool.RunTime.PoolItemSO dashVFXItem;

        private HumanDashPreparation _preparation;
        private HumanDashExecution _execution;
        private HumanDashHits _hits;
        internal HumanDashPreparation Preparation => _preparation ??= new HumanDashPreparation(this);
        internal HumanDashExecution Execution => _execution ??= new HumanDashExecution(this);
        internal HumanDashHits Hits => _hits ??= new HumanDashHits(this);
        internal float DashDistance => dashDistance;
        internal float DashDuration => dashDuration;
        internal float WarningWidth => warningWidth;
        internal float BoundaryPadding => boundaryPadding;
        internal float DamageMultiplier => damageMultiplier;
        internal ContactFilter2D WhatIsTarget => whatIsTarget;
        internal Chuh007Lib.ObjectPool.RunTime.PoolItemSO DashVFXItem => dashVFXItem;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Rush;
        public override bool UsesOwnerAttackAnimation => false;
        public override bool UsesPatternPresentationAnimation => false;
        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
            => Preparation.OnPreparePattern(owner, ct);
        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Execution.OnExecutePattern(owner, ct);
        internal DamageData GetDamage(Enemy owner, float multiplier) => GetAttackDamage(owner, multiplier);
        internal void PlaySound() => PlayAttackSound();
        internal void PlayVFX(Chuh007Lib.ObjectPool.RunTime.PoolItemSO item, Vector3 position, float scale, Vector2 direction)
            => PlayPooledVFX(item, position, scale, direction);
    }
}
