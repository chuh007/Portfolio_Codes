using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "SkeletonMageFallingSlowFieldPattern", menuName = "SO/Pattern/SkeletonMageFallingSlowFieldPattern", order = 0)]
    public class SkeletonMageFallingSlowFieldPatternSO : BasePatternSO
    {
        [Header("Projectile")]
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private Color projectileColor = Color.white;
        [SerializeField, Min(0f)] private float projectileScale = 1f;
        [SerializeField, Min(0.01f)] private float fallDuration = 0.35f;
        [SerializeField, Min(0f)] private float fallHeight = 4f;
        [SerializeField] private float spinDegrees = 180f;
        [SerializeField] private string projectileSortingLayerName;
        [SerializeField] private int projectileSortingOrder = 10;

        [Header("Impact")]
        [SerializeField, Min(0f)] private float impactRadius = 1f;
        [SerializeField, Min(0f)] private float damage = 6f;
        [SerializeField] private DamageType damageType = DamageType.Stone;
        [SerializeField] private ContactFilter2D whatIsTarget;

        [Header("Slow Field")]
        [SerializeField, Min(0f)] private float slowFieldRadius = 2f;
        [SerializeField, Min(0f)] private float slowFieldDuration = 4f;
        [SerializeField, Range(0.01f, 1f)] private float slowSpeedMultiplier = 0.5f;
        [SerializeField] private string moveSpeedStatName = "moveSpeed";
        [SerializeField] private Color slowFieldColor = new Color(0.35f, 0.55f, 1f, 0.35f);
        [SerializeField, Min(3)] private int slowFieldSegments = 48;
        [SerializeField] private string slowFieldSortingLayerName;
        [SerializeField] private int slowFieldSortingOrder = 2;

        private SkeletonMageFallSequence _sequence;
        private SkeletonMageFallWarning _warning;
        private SkeletonMageFallProjectile _projectile;
        private SkeletonMageFallImpact _impact;
        internal SkeletonMageFallSequence Sequence => _sequence ??= new SkeletonMageFallSequence(this);
        internal SkeletonMageFallWarning Warning => _warning ??= new SkeletonMageFallWarning(this);
        internal SkeletonMageFallProjectile Projectile => _projectile ??= new SkeletonMageFallProjectile(this);
        internal SkeletonMageFallImpact Impact => _impact ??= new SkeletonMageFallImpact(this);
        internal Sprite ProjectileSprite => projectileSprite;
        internal Color ProjectileColor => projectileColor;
        internal float ProjectileScale => projectileScale;
        internal float FallDuration => fallDuration;
        internal float FallHeight => fallHeight;
        internal float SpinDegrees => spinDegrees;
        internal string ProjectileSortingLayerName => projectileSortingLayerName;
        internal int ProjectileSortingOrder => projectileSortingOrder;
        internal float ImpactRadius => impactRadius;
        internal float Damage => damage;
        internal DamageType DamageType => damageType;
        internal ContactFilter2D WhatIsTarget => whatIsTarget;
        internal float SlowFieldRadius => slowFieldRadius;
        internal float SlowFieldDuration => slowFieldDuration;
        internal float SlowSpeedMultiplier => slowSpeedMultiplier;
        internal string MoveSpeedStatName => moveSpeedStatName;
        internal Color SlowFieldColor => slowFieldColor;
        internal int SlowFieldSegments => slowFieldSegments;
        internal string SlowFieldSortingLayerName => slowFieldSortingLayerName;
        internal int SlowFieldSortingOrder => slowFieldSortingOrder;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Circle;
        public override bool UsesOwnerAttackAnimation => false;
        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            Vector2 landingPosition = SkeletonMageFallingSlowFieldContext.GetLandingPosition(owner);
            SkeletonMageFallingSlowFieldContext.StorePreparedLandingPosition(owner, landingPosition);

            EntityMover mover = owner != null ? owner.GetCompo<EntityMover>() : null;
            mover?.StopImmediately();
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        internal float GetTelegraphDuration(float duration) => ResolveTelegraphDuration(duration);
    }
}
