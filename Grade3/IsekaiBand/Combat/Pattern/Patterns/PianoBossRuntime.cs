using System;
using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    public class PianoBossRuntime : MonoBehaviour, IEntityComponent, IAfterInitalize
    {
        [Header("Anchor")]
        [SerializeField] private bool lockToArenaTop = true;
        [SerializeField] private float topOffset = 2.15f;
        [SerializeField] private Vector2 fallbackArenaSize = new Vector2(18f, 10f);

        [Header("Visual")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private SpriteRenderer visualRenderer;
        [SerializeField] private Sprite phase1Sprite;
        [SerializeField] private Sprite phase2Sprite;

        [Header("Piano Floor")]
        [SerializeField] private string sortingLayerName = "GroundEffect";
        [SerializeField] private int floorSortingOrder = -50;
        [SerializeField, Min(7)] private int keyCount = 14;
        [SerializeField, Min(0)] private int sideExtraKeyCount = 1;
        [SerializeField, Min(0f)] private float bottomExtension = 1.4f;
        [SerializeField, Min(0f)] private float topExtension = 1.4f;
        [SerializeField, Min(0.1f)] private float passiveBeatInterval = 1.1f;
        [SerializeField, Min(0.1f)] private float orderedKeyPatternSpeedMultiplier = 3f;
        [SerializeField] private Color whiteKeyColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color blackKeyColor = new Color(0.02f, 0.02f, 0.025f, 1f);
        [SerializeField] private Color activeKeyColor = new Color(1f, 0.82f, 0.28f, 1f);
        [SerializeField] private Color dangerKeyColor = new Color(1f, 0.18f, 0.18f, 1f);

        private PianoBossPose _pose;
        private PianoFloorGeometry _floor;
        private PianoFloorVisuals _floorVisuals;
        private PianoFloorDecoration _floorDecoration;
        private PianoSpriteFactory _spriteFactory;
        private PianoKeyStrike _keyStrike;
        private PianoGlissando _glissando;
        private PianoFloorPatterns _floorPatterns;
        private PianoPassiveBeat _passiveBeat;
        private PianoPerformance _performance;
        private readonly PianoPatternCooldowns _cooldowns = new();

        internal PianoBossPose Pose => _pose ??= new PianoBossPose(this);
        internal PianoFloorGeometry Floor => _floor ??= new PianoFloorGeometry(this);
        internal PianoFloorVisuals FloorVisuals => _floorVisuals ??= new PianoFloorVisuals(this);
        internal PianoFloorDecoration FloorDecoration => _floorDecoration ??= new PianoFloorDecoration(this);
        internal PianoSpriteFactory SpriteFactory => _spriteFactory ??= new PianoSpriteFactory(this);
        internal PianoKeyStrike KeyStrike => _keyStrike ??= new PianoKeyStrike(this);
        internal PianoFloorPatterns FloorPatterns => _floorPatterns ??= new PianoFloorPatterns(this);
        internal PianoPassiveBeat PassiveBeat => _passiveBeat ??= new PianoPassiveBeat(this);
        internal PianoPerformance Performance => _performance ??= new PianoPerformance(this);
        internal Enemy Owner => Pose.Owner;
        internal bool LockToArenaTop => lockToArenaTop;
        internal float TopOffset => topOffset;
        internal Vector2 FallbackArenaSize => fallbackArenaSize;
        internal Transform VisualRoot { get => visualRoot; set => visualRoot = value; }
        internal SpriteRenderer VisualRenderer { get => visualRenderer; set => visualRenderer = value; }
        internal Sprite Phase1Sprite => phase1Sprite;
        internal Sprite Phase2Sprite => phase2Sprite;
        internal string SortingLayerName => sortingLayerName;
        internal int FloorSortingOrder => floorSortingOrder;
        internal int BaseKeyCount => Mathf.Max(7, keyCount);
        internal int ExtraKeyCount => Mathf.Max(0, sideExtraKeyCount);
        internal float BottomExtension => bottomExtension;
        internal float TopExtension => topExtension;
        internal float PassiveBeatInterval => passiveBeatInterval;
        internal float OrderedKeyPatternSpeedMultiplier => orderedKeyPatternSpeedMultiplier;
        internal Color WhiteKeyColor => whiteKeyColor;
        internal Color BlackKeyColor => blackKeyColor;
        internal Color ActiveKeyColor => activeKeyColor;
        internal Color DangerKeyColor => dangerKeyColor;
        public int KeyCount => BaseKeyCount + ExtraKeyCount * 2;

        public void Initialize(Entity entity) => EnsureBound(entity as Enemy);
        public void EnsureBound(Enemy owner) => Pose.EnsureBound(owner);
        public void AfterInitialize()
        {
            Pose.LockMovement();
            Pose.SnapToAnchor();
        }
        public void PlaceAtArenaTopForEncounter() => Pose.SnapToAnchor();

        private void Update()
        {
            if (Owner == null || Owner.IsDead) return;
            _cooldowns.UpdateCooldowns();
            Pose.LockMovement();
            Pose.SnapToAnchor();
            Performance.UpdatePerformancePulse();
            PassiveBeat.UpdatePianoFloor();
        }
        private void LateUpdate() => Pose.ApplyPhaseSprite();
        private void OnDestroy()
        {
            _passiveBeat?.StopPassivePattern();
            _floorVisuals?.DestroyFloor();
        }

        public IDisposable BeginPerformance() => Performance.BeginPerformance();
        public void PulsePerformance(float duration) => Performance.PulsePerformance(duration);
        public void EnsurePhase2Floor(Rect bounds, Sprite keySprite, DamageData floorDamage, LayerMask targetMask)
            => Floor.EnsurePhase2Floor(bounds, keySprite, floorDamage, targetMask);
        public bool IsPatternOnCooldown(object key) => _cooldowns.IsPatternOnCooldown(key);
        public void StartCooldown(object key, float duration) => _cooldowns.StartCooldown(key, duration);
        public int GetKeyIndex(Vector2 worldPosition) => Floor.GetKeyIndex(worldPosition);
        public Rect GetKeyRect(int keyIndex) => Floor.GetKeyRect(keyIndex);
        public UniTask StrikeKeysAsync(IReadOnlyList<int> keyIndices, float warningDuration, float activeDuration,
            DamageData damage, LayerMask targetMask, CancellationToken ct)
            => KeyStrike.StrikeKeysAsync(keyIndices, warningDuration, activeDuration, damage, targetMask, ct);
        public UniTask DropRhythmNoteAsync(int keyIndex, float fallDuration, DamageData damage, LayerMask targetMask, CancellationToken ct)
            => KeyStrike.DropRhythmNoteAsync(keyIndex, fallDuration, damage, targetMask, ct);
        public UniTask PlayTargetChordAsync(float warningDuration, float activeDuration, DamageData damage, LayerMask targetMask, CancellationToken ct)
            => FloorPatterns.PlayTargetChordAsync(warningDuration, activeDuration, damage, targetMask, ct);
        public UniTask PlayContiguousKeysAsync(int count, float warningDuration, float activeDuration, DamageData damage, LayerMask targetMask, CancellationToken ct)
            => FloorPatterns.PlayContiguousKeysAsync(count, warningDuration, activeDuration, damage, targetMask, ct);
        public UniTask PlayGlissandoAsync(int direction, float interval, float warningHoldDuration, float activeDuration,
            DamageData damage, LayerMask targetMask, CancellationToken ct)
            => (_glissando ??= new PianoGlissando(this)).PlayGlissandoAsync(direction, interval, warningHoldDuration, activeDuration, damage, targetMask, ct);
        public UniTask PlayRandomSingleNotesAsync(int count, float interval, DamageData damage, LayerMask targetMask, CancellationToken ct)
            => FloorPatterns.PlayRandomSingleNotesAsync(count, interval, damage, targetMask, ct);
    }
}
