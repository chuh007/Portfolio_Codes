using System.Threading;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "RushPattern", menuName = "SO/Pattern/RushPattern", order = 0)]
    public class RushPatternSO : BasePatternSO
    {
        private const float DashVFXInterval = 0.5f;
        private const float BoundaryDetectionPadding = 1.25f;
        private const float BoundaryStopPadding = 0.5f;
        private const float ObstacleStopPadding = 0.08f;
        private const float ObstacleMinimumLookAhead = 0.35f;
        private const float MinAnimationDuration = 0.01f;
        private int _dashObstacleLayerMask;

        [SerializeField] private float duration;
        [SerializeField] private float speed;
        [SerializeField] private float warningWidth = 1f;
        [SerializeField] private PoolItemSO dashVFXItem;
        [SerializeField] private string dashLoopSoundKey;
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
        [SerializeField] private bool normalizeDashAnimationTime = true;

        private Vector2 _preparedDirection;

        private RushAnimation _animation;
        private RushCollision _collision;
        private RushAnimation Animation => _animation ??= new RushAnimation(this);
        private RushCollision Collision => _collision ??= new RushCollision(this);
        internal int ObstacleLayerMask => _dashObstacleLayerMask;
        internal string WarningStateName => warningStateName;
        internal string LegacyWarningStateName => legacyWarningStateName;
        internal float AuthoredWarningDuration => authoredWarningDuration;
        internal string AttackStateName => attackStateName;
        internal string AttackBoolParameter => attackBoolParameter;
        internal string AttackBlendParameter => attackBlendParameter;
        internal string FallbackBlendParameter => fallbackBlendParameter;
        internal int AnimatorLayer => animatorLayer;
        internal float WarningWidth => warningWidth;
        internal ContactFilter2D WhatIsTarget => whatIsTarget;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Rush;
        public override bool UsesOwnerAttackAnimation => false;

        protected virtual void OnEnable()
        {
            _dashObstacleLayerMask = LayerMask.GetMask("Ground", "Water", "Wall");
        }

        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
                return;

            Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
            EntityMover mover = owner.GetCompo<EntityMover>();
            bool previousCanManualMove = mover != null && mover.CanManualMove;
            _preparedDirection = Vector2.zero;
            if (ct.IsCancellationRequested || !TryGetDirectionToTarget(owner, out _preparedDirection))
                return;

            // 경고 표시: rect 중심이 적 앞쪽에 오도록 오프셋
            float warnLength = speed * duration;
            Vector2 warnPos = (Vector2)owner.transform.position + _preparedDirection * (warnLength * 0.5f);
            float rotation = Vector2.SignedAngle(Vector2.up, _preparedDirection);

            var warning = poolManager.Pop(warningItem) as RectWarning;
            warning.Setup(warnPos, rotation, warningWidth, warnLength, true);
            mover?.StopImmediately();
            if (mover != null)
                mover.CanManualMove = false;
            else if (rb != null)
                rb.linearVelocity = Vector2.zero;

            RushAnimationState warningAnimation = useWarningAnimation
                ? Animation.BeginWarningAnimation(owner, warningDuration)
                : Animation.BeginAttackAnimation(owner, 0f);
            try
            {
                await warning.PlayAsync(warningDuration, ct);
            }
            finally
            {
                Animation.EndAttackAnimation(warningAnimation, true);
                if (mover != null)
                    mover.CanManualMove = previousCanManualMove;
            }
        }

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null || owner.IsDead)
                return;

            EntityRenderer render = owner.GetCompo<EntityRenderer>();
            EntityMover mover = owner.GetCompo<EntityMover>();
            Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();

            if (_preparedDirection.sqrMagnitude <= 0f && !TryGetDirectionToTarget(owner, out _preparedDirection))
                return;

            Vector2 dir = _preparedDirection;

            if (ct.IsCancellationRequested || owner.IsDead) return;

            render.FlipController(dir.x);
            bool wasKnockbackImmune = owner.IsKnockbackImmune;
            mover.CanManualMove = false;

            Collider2D bodyCollider = owner.GetComponent<Collider2D>();
            bool wasTrigger = false;
            if (bodyCollider != null)
            {
                wasTrigger = bodyCollider.isTrigger;
                bodyCollider.isTrigger = true;
            }

            try
            {
                owner.IsKnockbackImmune = true;

                DamageData damage = GetAttackDamage(owner, damageMultiplier);
                HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
                Vector2 previousPosition = owner.transform.position;

                PlayAttackSound();
                PlayDashVFX(owner, dir);
                float nextDashVFXTime = Time.time + DashVFXInterval;

                RushAnimationState dashAnimation = Animation.BeginAttackAnimation(owner, 0f, normalizeDashAnimationTime);
                object dashLoopPlaybackKey = StartDashLoopSound();
                float startTime = Time.time;
                bool stoppedDash = false;
                try
                {
                    while (startTime + duration > Time.time)
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, ct);
                        if (ShouldStopDash(owner, rb, ct))
                        {
                            stoppedDash = true;
                            break;
                        }

                        if (DashBoundaryUtility.TryGetStopPosition(
                                owner.transform.position,
                                dir,
                                BoundaryDetectionPadding,
                                BoundaryStopPadding,
                                out Vector2 stopPosition))
                        {
                            Collision.TryDamageTargets(owner, damage, previousPosition, stopPosition, dir, damagedTargets);
                            DashBoundaryUtility.MoveToStopPosition(owner, rb, stopPosition);
                            if (normalizeDashAnimationTime)
                                Animation.SampleAttackAnimation(dashAnimation, 1f);
                            break;
                        }

                        float normalizedTime = duration > Mathf.Epsilon
                            ? Mathf.Clamp01((Time.time - startTime) / duration)
                            : 1f;
                        float dashSpeed = speed * GetDashSpeedMultiplier(normalizedTime);
                        float obstacleCheckDistance = Mathf.Max(
                            ObstacleMinimumLookAhead,
                            dashSpeed * Mathf.Max(Time.deltaTime, MinAnimationDuration)
                            + ObstacleStopPadding);
                        if (Collision.TryGetObstacleStopPosition(
                                bodyCollider,
                                owner.transform.position,
                                dir,
                                obstacleCheckDistance,
                                out stopPosition))
                        {
                            Collision.TryDamageTargets(
                                owner,
                                damage,
                                previousPosition,
                                stopPosition,
                                dir,
                                damagedTargets);
                            rb.linearVelocity = Vector2.zero;
                            DashBoundaryUtility.MoveToStopPosition(owner, rb, stopPosition);
                            if (normalizeDashAnimationTime)
                                Animation.SampleAttackAnimation(dashAnimation, 1f);
                            break;
                        }

                        if (Time.time >= nextDashVFXTime)
                        {
                            PlayDashVFX(owner, dir);
                            nextDashVFXTime = Time.time + DashVFXInterval;
                        }

                        Vector2 currentPosition = owner.transform.position;
                        Collision.TryDamageTargets(owner, damage, previousPosition, currentPosition, dir, damagedTargets);
                        previousPosition = currentPosition;

                        if (normalizeDashAnimationTime)
                            Animation.SampleAttackAnimation(dashAnimation, normalizedTime);
                        rb.linearVelocity = dir * dashSpeed;
                    }

                    if (!stoppedDash && normalizeDashAnimationTime)
                        Animation.SampleAttackAnimation(dashAnimation, 1f);
                }
                finally
                {
                    StopDashLoopSound(dashLoopPlaybackKey);
                    Animation.EndAttackAnimation(dashAnimation, true);
                }
            }
            finally
            {
                if (bodyCollider != null)
                    bodyCollider.isTrigger = wasTrigger;

                if (owner != null)
                {
                    owner.IsKnockbackImmune = wasKnockbackImmune;

                    if (TryGetDirectionToTarget(owner, out Vector2 targetDir))
                        dir = targetDir;

                    if (render != null)
                        render.FlipController(dir.x);
                }

                if (rb != null)
                    rb.linearVelocity = Vector2.zero;

                if (mover != null)
                {
                    if (owner != null && owner.IsDead)
                    {
                        mover.StopImmediately();
                        mover.CanManualMove = false;
                    }
                    else
                    {
                        mover.CanManualMove = true;
                    }
                }
            }
        }

        protected virtual float GetDashSpeedMultiplier(float normalizedTime)
        {
            return 1f;
        }

        private void PlayDashVFX(Enemy owner, Vector2 direction)
        {
            if (owner == null) return;

            PlayPooledVFX(dashVFXItem, owner.transform.position, 1f, direction);
        }

        private object StartDashLoopSound()
        {
            if (string.IsNullOrWhiteSpace(dashLoopSoundKey))
                return null;

            var playbackKey = new object();
            Bus<SoundLoopStartEvent>.Raise(new SoundLoopStartEvent(
                playbackKey,
                dashLoopSoundKey));
            return playbackKey;
        }

        private static void StopDashLoopSound(object playbackKey)
        {
            if (playbackKey != null)
                Bus<SoundLoopStopEvent>.Raise(new SoundLoopStopEvent(playbackKey));
        }

        private static bool ShouldStopDash(Enemy owner, Rigidbody2D rb, CancellationToken ct)
        {
            if (!ct.IsCancellationRequested && owner != null && !owner.IsDead)
                return false;

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            return true;
        }

        private static bool TryGetDirectionToTarget(Enemy owner, out Vector2 direction)
        {
            direction = Vector2.zero;
            if (owner == null || owner.target == null)
                return false;

            direction = (owner.target.transform.position - owner.transform.position).normalized;
            return true;
        }

    }
}
