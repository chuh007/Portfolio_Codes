using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "GroupSlashPressurePattern", menuName = "SO/Pattern/GroupSlashPressurePattern", order = 0)]
    public class GroupSlashPressurePatternSO : BasePatternSO
    {
        private const string LogPrefix = "[GroupSlashPressurePattern]";
        [SerializeField] private PoolItemSO groupSlashItem;
        [SerializeField] private bool debugLog = true;
        [SerializeField, Min(1)] private int slashRepeatCount = 5;
        [SerializeField, Min(0f)] private float slashWarningDuration = 0.8f;
        [SerializeField, Min(0f)] private float slashInterval = 0.25f;
        [SerializeField, Min(0.1f)] private float warningRadius = 2.75f;
        [SerializeField, Min(0f)] private float aimOffsetMin = 0.75f;
        [SerializeField, Min(0f)] private float aimOffsetMax = 1.75f;
        [SerializeField, Min(0f)] private float mapPadding = 1.5f;
        [SerializeField, Min(0f)] private float targetSlashScale = 1f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.35f;
        [SerializeField, Min(0.01f)] private float damageTickInterval = 0.1f;

        private GroupSlashSequence _sequence;
        private GroupSlashAttack _attack;
        private GroupSlashAim _aim;
        internal GroupSlashSequence Sequence => _sequence ??= new GroupSlashSequence(this);
        internal GroupSlashAttack Attack => _attack ??= new GroupSlashAttack(this);
        internal GroupSlashAim Aim => _aim ??= new GroupSlashAim(this);
        internal PoolItemSO GroupSlashItem => groupSlashItem;
        internal bool DebugLog => debugLog;
        internal int SlashRepeatCount => slashRepeatCount;
        internal float SlashWarningDuration => slashWarningDuration;
        internal float SlashInterval => slashInterval;
        internal float WarningRadius => warningRadius;
        internal float AimOffsetMin => aimOffsetMin;
        internal float AimOffsetMax => aimOffsetMax;
        internal float MapPadding => mapPadding;
        internal float TargetSlashScale => targetSlashScale;
        internal float DamageMultiplier => damageMultiplier;
        internal float DamageTickInterval => damageTickInterval;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Summon;
        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            await base.OnPreparePattern(owner, ct);
        }

        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        internal DamageData GetDamage(Enemy owner, float multiplier) => GetAttackDamage(owner, multiplier);
        internal void LogWarning(Enemy owner, string message)
        {
            if (!debugLog) return;
            Debug.LogWarning($"{LogPrefix} {message}", owner);
        }
    }
}
