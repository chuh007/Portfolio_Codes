using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(
        fileName = "PianoBossHumanFallingNotesPattern",
        menuName = "SO/Pattern/PianoBoss Human/Falling Notes",
        order = 0)]
    public sealed class PianoBossHumanFallingNotesPatternSO : PianoBossHumanPatternBaseSO
    {
        [Header("Spawn Area")]
        [SerializeField, Min(1)] private int noteCount = 8;
        [SerializeField, Min(0f)] private float minimumSpawnRadius = 1.5f;
        [SerializeField, Min(0f)] private float maximumSpawnRadius = 5f;
        [SerializeField, Range(0f, 1f)] private float angularJitter = 0.35f;
        [SerializeField, Min(0f)] private float arenaPadding = 0.6f;
        [SerializeField] private Vector2 fallbackArenaSize = new Vector2(18f, 10f);

        [Header("Falling Notes")]
        [SerializeField] private Color fallingNoteColor = Color.white;
        [SerializeField, Min(0.05f)] private float fallingNoteScale = 0.8f;
        [SerializeField, Min(0f)] private float fallHeight = 5f;
        [SerializeField, Min(0.01f)] private float fallDuration = 0.45f;
        [SerializeField, Min(0f)] private float fallStaggerInterval = 0.06f;
        [SerializeField] private float spinDegrees = 240f;

        [Header("Impact")]
        [SerializeField, Min(0.05f)] private float impactRadius = 0.8f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.7f;

        [Header("Impact Shockwave")]
        [SerializeField, Min(0.05f)] private float shockwaveRadius = 0.95f;
        [SerializeField, Min(0.08f)] private float shockwaveDuration = 0.32f;
        [SerializeField] private Color shockwaveColor = new Color(0.72f, 0.18f, 1f, 0.72f);
        [SerializeField] private int shockwaveSortingOrder = 11;

        private FallingNotesSequence _sequence;
        private FallingNotesLanding _landing;
        private FallingNotesVisual _visual;
        private FallingNotesImpact _impact;
        internal FallingNotesSequence Sequence => _sequence ??= new FallingNotesSequence(this);
        internal FallingNotesLanding Landing => _landing ??= new FallingNotesLanding(this);
        internal FallingNotesVisual Visual => _visual ??= new FallingNotesVisual(this);
        internal FallingNotesImpact Impact => _impact ??= new FallingNotesImpact(this);
        internal int NoteCount => noteCount;
        internal float MinimumSpawnRadius => minimumSpawnRadius;
        internal float MaximumSpawnRadius => maximumSpawnRadius;
        internal float AngularJitter => angularJitter;
        internal float ArenaPadding => arenaPadding;
        internal Vector2 FallbackArenaSize => fallbackArenaSize;
        internal Color FallingNoteColor => fallingNoteColor;
        internal float FallingNoteScale => fallingNoteScale;
        internal float FallHeight => fallHeight;
        internal float FallDuration => fallDuration;
        internal float FallStaggerInterval => fallStaggerInterval;
        internal float SpinDegrees => spinDegrees;
        internal float ImpactRadius => impactRadius;
        internal float DamageMultiplier => damageMultiplier;
        internal float ShockwaveRadius => shockwaveRadius;
        internal float ShockwaveDuration => shockwaveDuration;
        internal Color ShockwaveColor => shockwaveColor;
        internal int ShockwaveSortingOrder => shockwaveSortingOrder;

        public override bool UsesOwnerAttackAnimation => false;
        internal LayerMask TargetMask => whatIsTarget;
        internal int VisualOrder => visualSortingOrder;
        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            StopOwner(owner);

            Vector2[] landingPositions = Landing.CreateLandingPositions(owner);
            PianoBossHumanFallingNotesContext context = FallingNotesLanding.GetOrAddContext(owner);
            context.Store(landingPositions);
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        internal DamageData GetNoteDamage(Enemy owner, float multiplier) => CreateNoteDamage(owner, multiplier);
        internal Sprite NoteSprite(int index) => GetNoteSprite(index);
        internal float GetTelegraphDuration(float duration) => ResolveTelegraphDuration(duration);
    }
}
