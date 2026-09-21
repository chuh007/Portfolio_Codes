using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(
        fileName = "PianoBossHumanClosingRingPattern",
        menuName = "SO/Pattern/PianoBoss Human/Closing Note Ring",
        order = 4)]
    public sealed class PianoBossHumanClosingRingPatternSO : PianoBossHumanPatternBaseSO
    {
        private const int DirectionCount = 4;
        [Header("Closing Note Ring")]
        [SerializeField, Min(8)] private int noteCount = 40;
        [SerializeField, Min(0.5f)] private float ringRadius = 5.8f;
        [SerializeField, Range(20f, 140f)] private float openSectorAngle = 90f;
        [SerializeField, Min(0.05f)] private float approachSpeed = 2.8f;
        [SerializeField, Min(0f)] private float centerDespawnRadius = 0.25f;
        [SerializeField, Min(0.05f)] private float projectileScale = 0.64f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.45f;
        [SerializeField, Min(0f)] private float patternLockDuration = 0.85f;

        [Header("Visual")]
        [SerializeField] private Color projectileColor = Color.white;
        [SerializeField] private Color trailColor = new Color(0.72f, 0.18f, 1f, 0.75f);
        [SerializeField, Min(0.05f)] private float trailTime = 0.28f;
        [SerializeField, Min(0.01f)] private float trailStartWidth = 0.2f;
        [SerializeField, Min(0f)] private float trailEndWidth = 0.02f;

        private ClosingRingSpawn _spawn;
        internal ClosingRingSpawn Spawn => _spawn ??= new ClosingRingSpawn(this);
        internal int NoteCount => noteCount;
        internal float RingRadius => ringRadius;
        internal float OpenSectorAngle => openSectorAngle;
        internal float ApproachSpeed => approachSpeed;
        internal float CenterDespawnRadius => centerDespawnRadius;
        internal float ProjectileScale => projectileScale;
        internal float DamageMultiplier => damageMultiplier;
        internal float PatternLockDuration => patternLockDuration;
        internal Color ProjectileColor => projectileColor;
        internal Color TrailColor => trailColor;
        internal float TrailTime => trailTime;
        internal float TrailStartWidth => trailStartWidth;
        internal float TrailEndWidth => trailEndWidth;

        internal int VisualOrder => visualSortingOrder;
        internal LayerMask TargetMask => whatIsTarget;
        internal DamageData GetDamage(Enemy owner, float multiplier) => CreateNoteDamage(owner, multiplier);
        internal Sprite NoteSprite(int index) => GetNoteSprite(index);

        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            StopOwner(owner);

            Vector2 center = PianoBossHumanClosingRingContext.GetTargetCenter(owner);
            int openDirection = Random.Range(0, DirectionCount);
            PianoBossHumanClosingRingContext context = PianoBossHumanClosingRingContext.GetOrAddContext(owner);
            context.Store(center, openDirection);

            return UniTask.CompletedTask;
        }

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
                return;

            if (!PianoBossHumanClosingRingContext.TryConsumePlan(owner, out Vector2 center, out int openDirection))
            {
                center = PianoBossHumanClosingRingContext.GetTargetCenter(owner);
                openDirection = Random.Range(0, DirectionCount);
            }

            Spawn.SpawnClosingRing(owner, center, openDirection);

            if (patternLockDuration > 0f)
                await UniTask.WaitForSeconds(patternLockDuration, cancellationToken: ct);
        }
    }
}
