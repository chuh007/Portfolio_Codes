using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(
        fileName = "PianoBossDnaHelixPattern",
        menuName = "SO/Pattern/PianoBoss/DNA Helix",
        order = 2)]
    public sealed class PianoBossDnaHelixPatternSO : PianoBossPatternBaseSO
    {
        [Header("DNA Helix Projectiles")]
        [SerializeField] private Sprite helixProjectileSprite;
        [SerializeField, Min(1)] private int helixCount = 4;
        [SerializeField, Min(1)] private int pairCount = 14;
        [SerializeField, Min(0.02f)] private float spawnInterval = 0.18f;
        [SerializeField, Min(0.05f)] private float travelDuration = 4.4f;
        [SerializeField, Min(0f)] private float projectileLifeTime = 4.55f;
        [SerializeField, Min(0f)] private float horizontalAmplitude = 1.45f;
        [SerializeField, Min(0f)] private float helixEdgePadding = 0.35f;
        [SerializeField, Min(0f)] private float helixTurns = 2.25f;
        [SerializeField, Min(0f)] private float cameraEdgePadding = 1.2f;
        [SerializeField, Min(0.05f)] private float projectileScale = 0.3f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.32f;

        [Header("Strand Visuals")]
        [SerializeField] private Color strandAColor = new(0.32f, 0.9f, 1f, 0.96f);
        [SerializeField] private Color strandBColor = new(0.88f, 0.34f, 1f, 0.96f);
        [SerializeField, Min(0.05f)] private float trailTime = 0.28f;
        [SerializeField, Min(0.01f)] private float trailStartWidth = 0.15f;
        [SerializeField, Min(0f)] private float trailEndWidth = 0.01f;

        private DnaHelixSequence _sequence;
        private DnaHelixSpawn _spawn;
        internal DnaHelixSequence Sequence => _sequence ??= new DnaHelixSequence(this);
        internal DnaHelixSpawn Spawn => _spawn ??= new DnaHelixSpawn(this);
        internal Sprite HelixProjectileSprite => helixProjectileSprite;
        internal int HelixCount => helixCount;
        internal int PairCount => pairCount;
        internal float SpawnInterval => spawnInterval;
        internal float TravelDuration => travelDuration;
        internal float ProjectileLifeTime => projectileLifeTime;
        internal float HorizontalAmplitude => horizontalAmplitude;
        internal float HelixEdgePadding => helixEdgePadding;
        internal float HelixTurns => helixTurns;
        internal float CameraEdgePadding => cameraEdgePadding;
        internal float ProjectileScale => projectileScale;
        internal float DamageMultiplier => damageMultiplier;
        internal Color StrandAColor => strandAColor;
        internal Color StrandBColor => strandBColor;
        internal float TrailTime => trailTime;
        internal float TrailStartWidth => trailStartWidth;
        internal float TrailEndWidth => trailEndWidth;

        internal float FloorMultiplier => FloorDamageMultiplier;
        internal int VisualOrder => visualSortingOrder;
        internal LayerMask TargetMask => whatIsTarget;
        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct) => Sequence.OnExecutePattern(owner, ct);
        internal PianoBossRuntime RuntimeFor(Enemy owner) => GetRuntime(owner);
        internal void EnsureFloor(Enemy owner, PianoBossRuntime runtime, DamageData damage) => EnsurePianoFloor(owner, runtime, damage);
        internal DamageData GetDamage(Enemy owner, float multiplier) => CreateDamage(owner, multiplier);
        internal Rect ArenaBounds(Enemy owner) => GetArenaBounds(owner);
        internal Sprite NoteSprite(int index) => GetNoteSprite(index);
        internal Sprite SelectSprite(Sprite primary, Sprite fallback) => ResolveSprite(primary, fallback);
    }
}
