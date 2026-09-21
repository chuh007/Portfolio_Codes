using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "Phase2LaneSweepPattern", menuName = "SO/Pattern/Phase2LaneSweepPattern", order = 0)]
    public class Phase2LaneSweepPatternSO : BasePatternSO
    {
        [SerializeField, Min(1)] private int warningRepeatCount = 5;
        [SerializeField, Min(0f)] private float mapPadding = 0f;
        [SerializeField, Min(0f)] private float cameraPadding = 1f;
        [SerializeField, Min(0f)] private float cameraZoomDuration = 0.6f;
        [SerializeField, Min(0f)] private float cameraRestoreDuration = 0.45f;
        [SerializeField, Min(0f)] private float sweepInterval = 1f;
        [SerializeField] private PoolItemSO laneSweepVFXItem;
        [SerializeField, Min(0f)] private float laneSweepVFXScale = 1f;
        [SerializeField] private Vector2 laneSweepDirection = Vector2.left;
        [SerializeField, Min(5)] private int slashCountPerLane = 6;
        [SerializeField, Min(0f)] private float slashSpawnDelay = 0.08f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.35f;
        [SerializeField, Min(0.01f)] private float damageTickInterval = 0.1f;
        [SerializeField, Min(0f)] private float returnDelayAfterLastAttack = 2f;
        [SerializeField] private PatternVanishPresentation vanishPresentation = new();

        private LaneSweepSequence _sequence;
        private LaneSweepPlan _plan;
        private LaneSweepWarnings _warnings;
        private LaneSweepSlashes _slashes;
        private LaneSweepPresentation _presentation;
        private LaneSweepCamera _camera;
        private LaneSweepBlend _blend;
        internal LaneSweepSequence Sequence => _sequence ??= new LaneSweepSequence(this);
        internal LaneSweepPlan Plan => _plan ??= new LaneSweepPlan(this);
        internal LaneSweepWarnings Warnings => _warnings ??= new LaneSweepWarnings(this);
        internal LaneSweepSlashes Slashes => _slashes ??= new LaneSweepSlashes(this);
        internal LaneSweepPresentation Presentation => _presentation ??= new LaneSweepPresentation(this);
        internal LaneSweepCamera Camera => _camera ??= new LaneSweepCamera(this);
        internal LaneSweepBlend Blend => _blend ??= new LaneSweepBlend(this);
        internal int WarningRepeatCount => warningRepeatCount;
        internal float MapPadding => mapPadding;
        internal float CameraPadding => cameraPadding;
        internal float CameraZoomDuration => cameraZoomDuration;
        internal float CameraRestoreDuration => cameraRestoreDuration;
        internal float SweepInterval => sweepInterval;
        internal PoolItemSO LaneSweepVFXItem => laneSweepVFXItem;
        internal float LaneSweepVFXScale => laneSweepVFXScale;
        internal Vector2 LaneSweepDirection => laneSweepDirection;
        internal int SlashCountPerLane => slashCountPerLane;
        internal float SlashSpawnDelay => slashSpawnDelay;
        internal float DamageMultiplier => damageMultiplier;
        internal float DamageTickInterval => damageTickInterval;
        internal float ReturnDelayAfterLastAttack => returnDelayAfterLastAttack;
        internal PatternVanishPresentation VanishPresentation => vanishPresentation;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Circle;
        public override bool UsesOwnerAttackAnimation => false;
        internal int SweepCount => Mathf.Max(1, warningRepeatCount);
        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnPreparePattern(owner, ct);
        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        internal UniTask PrepareFallback(Enemy owner, CancellationToken ct) => base.OnPreparePattern(owner, ct);
        internal DamageData GetDamage(Enemy owner, float multiplier) => GetAttackDamage(owner, multiplier);
    }
}
