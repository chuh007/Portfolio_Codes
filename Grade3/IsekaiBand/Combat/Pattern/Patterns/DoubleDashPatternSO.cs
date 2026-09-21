using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "DoubleDashPattern", menuName = "SO/Pattern/DoubleDashPattern", order = 0)]
    public class DoubleDashPatternSO : BasePatternSO
    {
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float speed = 45f;
        [SerializeField] private float warningWidth = 1f;
        [SerializeField] private float dashInterval = 0.12f;
        [SerializeField] private float secondWarningDuration = 0.25f;
        [SerializeField] private PoolItemSO dashVFXItem;
        [SerializeField, Min(0f)] private float damageMultiplier = 1f;
        [SerializeField] private ContactFilter2D whatIsTarget;

        [Header("Animation")]
        [SerializeField] private bool useWarningAnimation;
        [SerializeField] private string warningStateName = "AttackReady";
        [SerializeField] private string legacyWarningStateName = "AttackWarning";
        [SerializeField, Min(0.01f)] private float authoredWarningDuration = 1f;
        [SerializeField] private string attackStateName = "Attack";
        [SerializeField] private string attackBoolParameter = "ATTACK";
        [SerializeField] private string attackBlendParameter = "AttackKey";
        [SerializeField] private string fallbackBlendParameter = "Blend";
        [SerializeField] private int animatorLayer = 0;

        private DoubleDashSequence _sequence;
        private DoubleDashWarnings _warnings;
        private DoubleDashMovement _movement;
        private DoubleDashAnimation _animation;
        private DoubleDashTargets _targets;
        internal DoubleDashSequence Sequence => _sequence ??= new DoubleDashSequence(this);
        internal DoubleDashWarnings Warnings => _warnings ??= new DoubleDashWarnings(this);
        internal DoubleDashMovement Movement => _movement ??= new DoubleDashMovement(this);
        internal DoubleDashAnimation Animation => _animation ??= new DoubleDashAnimation(this);
        internal DoubleDashTargets Targets => _targets ??= new DoubleDashTargets(this);
        internal float Duration => duration;
        internal float Speed => speed;
        internal float WarningWidth => warningWidth;
        internal float DashInterval => dashInterval;
        internal float SecondWarningDuration => secondWarningDuration;
        internal PoolItemSO DashVFXItem => dashVFXItem;
        internal float DamageMultiplier => damageMultiplier;
        internal ContactFilter2D WhatIsTarget => whatIsTarget;
        internal bool UseWarningAnimation => useWarningAnimation;
        internal string WarningStateName => warningStateName;
        internal string LegacyWarningStateName => legacyWarningStateName;
        internal float AuthoredWarningDuration => authoredWarningDuration;
        internal string AttackStateName => attackStateName;
        internal string AttackBoolParameter => attackBoolParameter;
        internal string AttackBlendParameter => attackBlendParameter;
        internal string FallbackBlendParameter => fallbackBlendParameter;
        internal int AnimatorLayer => animatorLayer;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Rush;
        public override bool UsesOwnerAttackAnimation => false;
        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnPreparePattern(owner, ct);
        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        protected virtual float GetDashSpeedMultiplier(float normalizedTime)
        {
            return 1f;
        }
        internal float GetSpeedMultiplier(float normalizedTime) => GetDashSpeedMultiplier(normalizedTime);
        internal DamageData GetDamage(Enemy owner, float multiplier) => GetAttackDamage(owner, multiplier);
        internal void PlaySound() => PlayAttackSound();
        internal void PlayVFX(PoolItemSO item, Vector3 position, float scale, Vector2 direction)
            => PlayPooledVFX(item, position, scale, direction);
    }
}
