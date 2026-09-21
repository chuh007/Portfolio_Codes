using System;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Enemies.Boss;
using _Work.CHUH.Code.EntityPlus;
using _Work.CHUH.Code.Visual;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern
{
    public enum PatternAttackType
    {
        Auto = -1,
        Rush = 0,
        Circle = 1,
        FireBullet = 2,
        Summon = 3
    }

    public enum PatternUseMode
    {
        Full,
        PrepareOnly,
        ExecuteOnly
    }

    public enum PatternVFXDirectionSource
    {
        OwnerFacing,
        Target,
        Custom
    }

    /// <summary>
    /// 보스가 사용할 패턴의 기반
    /// AttackCompo가 패턴들을 리스트로 든다.
    /// </summary>
    public abstract class BasePatternSO : ScriptableObject
    {
        public PoolManagerSO poolManager;
        public PoolItemSO warningItem;

        [Header("Animation")]
        [SerializeField] private PatternAttackType attackType = PatternAttackType.Auto;
        [SerializeField] private string executeAnimationStateName;

        [Header("Attack VFX")]
        [SerializeField] private PoolItemSO attackVFXItem;
        [SerializeField] private float attackVFXScale = 1f;
        [SerializeField] private PatternVFXDirectionSource attackVFXDirectionSource = PatternVFXDirectionSource.OwnerFacing;
        [SerializeField] private Vector2 attackVFXCustomDirection = Vector2.right;

        [Header("Audio")]
        [SerializeField] private string attackSoundKey;
        [SerializeField, Min(0f)] private float attackSoundVolumeMultiplier = 1f;

        [Header("Timing")]
        [SerializeField] protected float warningDuration = 1f;
        [SerializeField] private float recoveryDelay = 2f;

        [Header("Selection")]
        [Tooltip("사용 후 다시 선택되기까지 완료해야 하는 다른 패턴 횟수")]
        [SerializeField, Min(0)] private int selectionCooldownPatterns;

        protected virtual PatternAttackType DefaultAttackType => PatternAttackType.Rush;
        public PatternAttackType AttackType => attackType == PatternAttackType.Auto ? DefaultAttackType : attackType;
        public int BlendTreeValue => (int)AttackType;
        public string ExecuteAnimationStateName => executeAnimationStateName;
        public virtual bool UsesOwnerAttackAnimation => true;
        public virtual bool UsesPatternPresentationAnimation => true;
        public virtual bool CanReplaceRepeatedSelection => true;
        public float WarningDuration => warningDuration;
        public float RecoveryDelay => recoveryDelay;
        public int SelectionCooldownPatterns => selectionCooldownPatterns;
        public Action OnWarringEnd;

        public virtual BasePatternSO ResolveSelectionPattern(Enemy owner)
        {
            return this;
        }

        public async UniTask UsePattern(Enemy owner, CancellationToken ct, PatternUseMode mode = PatternUseMode.Full)
        {
            if (owner is Boss boss && !boss.IsPatternExecutionEnabled)
                return;

            switch (mode)
            {
                case PatternUseMode.Full:
                    await RunWithKnockbackImmunity(owner, () => OnPreparePattern(owner, ct));
                    OnWarringEnd?.Invoke();

                    if (ct.IsCancellationRequested) return;

                    PlayAttackVFX(owner);
                    await RunWithKnockbackImmunity(owner, () => OnExecutePattern(owner, ct));
                    await PlayRecoveryDelay(ct);
                    break;

                case PatternUseMode.PrepareOnly:
                    await RunWithKnockbackImmunity(owner, () => OnPreparePattern(owner, ct));

                    if (ct.IsCancellationRequested) return;

                    OnWarringEnd?.Invoke();
                    break;

                case PatternUseMode.ExecuteOnly:
                    PlayAttackVFX(owner);
                    await RunWithKnockbackImmunity(owner, () => OnExecutePattern(owner, ct));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        protected virtual async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            if (warningDuration <= 0f) return;
            await UniTask.WaitForSeconds(warningDuration, cancellationToken: ct);
        }

        protected abstract UniTask OnExecutePattern(Enemy owner, CancellationToken ct);

        /// <summary>
        /// 경고와 공격 연출을 동시에 재생할 때 사용할 시간을 반환한다.
        /// 경고가 활성화되어 있으면 경고 종료 시점이 공격 시점이 되도록 경고 시간을 우선한다.
        /// </summary>
        protected float ResolveTelegraphDuration(float fallbackDuration)
        {
            float duration = warningDuration > 0f ? warningDuration : fallbackDuration;
            return Mathf.Max(0.01f, duration);
        }

        private async UniTask RunWithKnockbackImmunity(Enemy owner, Func<UniTask> action)
        {
            if (owner == null || AttackType != PatternAttackType.Rush)
            {
                await action();
                return;
            }

            bool wasKnockbackImmune = owner.IsKnockbackImmune;
            owner.IsKnockbackImmune = true;

            try
            {
                await action();
            }
            finally
            {
                owner.IsKnockbackImmune = wasKnockbackImmune;
            }
        }

        protected DamageData GetAttackDamage(Enemy owner, float multiplier = 1f)
        {
            EntityStat stat = owner != null ? owner.GetCompo<EntityStat>() : null;
            StatSO damageStat = null;
            stat?.TryGetStatByName("AttackDamage", out damageStat);

            float baseDamage = damageStat != null ? damageStat.Value : 0f;
            return new DamageData(baseDamage * Mathf.Max(0f, multiplier));
        }

        protected void PlayAttackSound()
        {
            if (string.IsNullOrWhiteSpace(attackSoundKey))
                return;

            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                attackSoundKey,
                SoundType.SFX,
                attackSoundVolumeMultiplier));
        }

        protected void PlayPooledVFX(PoolItemSO item, Vector3 position, float scale, Vector2 direction)
        {
            if (poolManager == null || item == null) return;

            var pooled = poolManager.Pop(item);
            if (pooled is IPooledVFX vfx)
            {
                vfx.Play(position, scale, direction);
                return;
            }

            Debug.LogWarning($"[{nameof(BasePatternSO)}] {item.name} prefab needs IPooledVFX.");
            if (pooled != null)
                poolManager.Push(pooled);
        }

        private void PlayAttackVFX(Enemy owner)
        {
            if (owner == null) return;

            PlayPooledVFX(attackVFXItem, owner.transform.position, attackVFXScale, GetAttackVFXDirection(owner));
        }

        private Vector2 GetAttackVFXDirection(Enemy owner)
        {
            switch (attackVFXDirectionSource)
            {
                case PatternVFXDirectionSource.OwnerFacing:
                    EntityRenderer renderer = owner.GetCompo<EntityRenderer>();
                    float facingDirection = renderer != null ? renderer.FacingDirection : owner.transform.localScale.x;
                    return new Vector2(Mathf.Sign(facingDirection), 0f);

                case PatternVFXDirectionSource.Target:
                    if (owner.target == null)
                        return Vector2.right;

                    Vector2 direction = owner.target.transform.position - owner.transform.position;
                    return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;

                case PatternVFXDirectionSource.Custom:
                    return attackVFXCustomDirection.sqrMagnitude > 0.001f
                        ? attackVFXCustomDirection.normalized
                        : Vector2.right;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask PlayRecoveryDelay(CancellationToken ct)
        {
            if (recoveryDelay <= 0f) return;
            await UniTask.WaitForSeconds(recoveryDelay, cancellationToken: ct);
        }
    }

}
