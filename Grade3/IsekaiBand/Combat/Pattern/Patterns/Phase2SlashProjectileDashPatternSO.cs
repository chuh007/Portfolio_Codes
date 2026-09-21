using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "Phase2SlashProjectileDashPattern", menuName = "SO/Pattern/Phase2SlashProjectileDashPattern", order = 0)]
    public class Phase2SlashProjectileDashPatternSO : BasePatternSO
    {
        [Header("Slash Projectile")]
        [SerializeField] private PoolItemSO slashProjectileItem;
        [SerializeField, Min(0f)] private float initialDelay = 0.2f;
        [SerializeField, Min(1)] private int projectileCount = 3;
        [SerializeField, Min(0.01f)] private float fireInterval = 0.5f;
        [SerializeField, Min(0f)] private float projectileWarningDuration = 0.12f;
        [SerializeField, Min(0f)] private float projectileWarningLength = 12f;
        [SerializeField, Min(0f)] private float projectileSpawnOffset = 1f;
        [SerializeField, Min(0f)] private float projectileScale = 0.75f;
        [SerializeField, Min(0f)] private float projectileSpeed = 18f;
        [SerializeField, Min(0.01f)] private float projectileLifeTime = 0.65f;
        [SerializeField, Min(0f)] private float projectileHitRadius = 0.45f;
        [SerializeField, Min(0f)] private float projectileDamageMultiplier = 0.35f;

        [Header("Dash")]
        [SerializeField, Min(0f)] private float dashDelay = 0.2f;
        [SerializeField, Min(0f)] private float dashWarningDuration = 0.35f;
        [SerializeField, Min(0f)] private float dashWarningWidth = 2.4f;
        [SerializeField, Min(0.01f)] private float dashDuration = 0.22f;
        [SerializeField, Min(0f)] private float dashSpeed = 35f;
        [SerializeField, Min(0f)] private float dashHitRadius = 1.2f;
        [SerializeField, Min(0f)] private float dashDamageMultiplier = 1f;
        [SerializeField] private PoolItemSO dashVFXItem;
        [SerializeField] private ContactFilter2D whatIsTarget;

        [Header("Animation")]
        [SerializeField] private string fullAttackStateName = "FullAttack";
        [SerializeField] private AnimationClip fullAttackClip;
        [SerializeField] private string attackStateName = "Attack";
        [SerializeField] private string idleStateName = "Idle";
        [SerializeField] private string attackBoolParameter = "ATTACK";
        [SerializeField] private string attackBlendParameter = "AttackKey";
        [SerializeField] private string fallbackBlendParameter = "Blend";
        [SerializeField] private int animatorLayer = 0;

        private Phase2SlashExecution _execution;
        private Phase2SlashWarnings _warnings;
        private Phase2SlashProjectiles _projectiles;
        private Phase2SlashDash _dash;
        private Phase2SlashFullAttack _fullAttack;
        private Phase2SlashAnimation _animation;
        private Phase2SlashTimeline _timeline;
        private Phase2SlashTargets _targets;

        internal Phase2SlashExecution Execution => _execution ??= new Phase2SlashExecution(this);
        internal Phase2SlashWarnings Warnings => _warnings ??= new Phase2SlashWarnings(this);
        internal Phase2SlashProjectiles Projectiles => _projectiles ??= new Phase2SlashProjectiles(this);
        internal Phase2SlashDash Dash => _dash ??= new Phase2SlashDash(this);
        internal Phase2SlashFullAttack FullAttack => _fullAttack ??= new Phase2SlashFullAttack(this);
        internal Phase2SlashAnimation Animation => _animation ??= new Phase2SlashAnimation(this);
        internal Phase2SlashTimeline Timeline => _timeline ??= new Phase2SlashTimeline(this);
        internal Phase2SlashTargets Targets => _targets ??= new Phase2SlashTargets(this);
        internal PoolItemSO SlashProjectileItem => slashProjectileItem;
        internal float InitialDelay => initialDelay;
        internal int ProjectileCount => projectileCount;
        internal float FireInterval => fireInterval;
        internal float ProjectileWarningDuration => projectileWarningDuration;
        internal float ProjectileWarningLength => projectileWarningLength;
        internal float ProjectileSpawnOffset => projectileSpawnOffset;
        internal float ProjectileScale => projectileScale;
        internal float ProjectileSpeed => projectileSpeed;
        internal float ProjectileLifeTime => projectileLifeTime;
        internal float ProjectileHitRadius => projectileHitRadius;
        internal float ProjectileDamageMultiplier => projectileDamageMultiplier;
        internal float DashDelay => dashDelay;
        internal float DashWarningDuration => dashWarningDuration;
        internal float DashWarningWidth => dashWarningWidth;
        internal float DashDuration => dashDuration;
        internal float DashSpeed => dashSpeed;
        internal float DashHitRadius => dashHitRadius;
        internal float DashDamageMultiplier => dashDamageMultiplier;
        internal PoolItemSO DashVFXItem => dashVFXItem;
        internal ContactFilter2D WhatIsTarget => whatIsTarget;
        internal string FullAttackStateName => fullAttackStateName;
        internal AnimationClip FullAttackClip => fullAttackClip;
        internal string AttackStateName => attackStateName;
        internal string IdleStateName => idleStateName;
        internal string AttackBoolParameter => attackBoolParameter;
        internal string AttackBlendParameter => attackBlendParameter;
        internal string FallbackBlendParameter => fallbackBlendParameter;
        internal int AnimatorLayer => animatorLayer;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Rush;
        public override bool UsesOwnerAttackAnimation => false;

        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            owner.GetCompo<EntityMover>()?.StopImmediately();
            await base.OnPreparePattern(owner, ct);
        }

        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Execution.OnExecutePattern(owner, ct);
        internal DamageData GetDamage(Enemy owner, float multiplier) => GetAttackDamage(owner, multiplier);
        internal void PlaySound() => PlayAttackSound();
        internal void PlayVFX(PoolItemSO item, Vector3 position, float scale, Vector2 direction)
            => PlayPooledVFX(item, position, scale, direction);
    }
}
