using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "MiddleBossLobRockPattern", menuName = "SO/Pattern/MiddleBossLobRockPattern", order = 0)]
    public class MiddleBossLobRockPatternSO : BasePatternSO
    {
        [Header("Rock")]
        [SerializeField] private GameObject rockPrefab;
        [SerializeField] private Sprite rockSprite;
        [SerializeField] private Color rockColor = Color.white;
        [SerializeField, Min(0f)] private float rockScale = 1f;
        [SerializeField] private int rockSortingOrder = 5;
        [SerializeField, Min(0)] private int randomRockCount = 3;
        [SerializeField, Min(0f)] private float launchInterval = 0.12f;
        [SerializeField, Min(0.01f)] private float travelDuration = 0.9f;
        [SerializeField, Min(0f)] private float arcHeight = 2.5f;
        [SerializeField] private float launchHeight = 0.5f;
        [SerializeField] private float spinDegrees = 360f;

        [Header("Impact")]
        [SerializeField, Min(0f)] private float randomLandingRadius = 3.5f;
        [SerializeField, Min(0f)] private float randomLandingMinDistance = 1.25f;
        [SerializeField, Min(0f)] private float impactRadius = 2f;
        [SerializeField, Min(0f)] private float damageMultiplier = 1f;
        [SerializeField] private DamageType damageType = DamageType.Physical;
        [SerializeField] private ContactFilter2D whatIsTarget;
        [SerializeField] private string impactSoundKey = SoundKeys.RockImpact;

        [Header("Impact VFX")]
        [SerializeField] private Sprite[] impactSprites;
        [SerializeField, Min(0.01f)] private float impactFrameInterval = 0.05f;
        [SerializeField, Min(0f)] private float impactEffectScale = 1.5f;
        [SerializeField] private string impactSortingLayerName;
        [SerializeField] private int impactSortingOrder = 11;

        [Header("Attack Pose")]
        [SerializeField] private string attackStateName = "Attack";
        [SerializeField] private string attackBoolParameter = "ATTACK";
        [SerializeField, Range(0f, 1f)] private float attackFrameNormalizedTime = 0f;
        [SerializeField, Min(0.01f)] private float attackPoseDuration = 0.08f;
        [SerializeField] private int animatorLayer = 0;

        private LobRockSequence _sequence;
        private LobRockLanding _landing;
        private LobRockProjectile _projectile;
        private LobRockImpact _impact;
        private LobRockPose _pose;
        internal LobRockSequence Sequence => _sequence ??= new LobRockSequence(this);
        internal LobRockLanding Landing => _landing ??= new LobRockLanding(this);
        internal LobRockProjectile Projectile => _projectile ??= new LobRockProjectile(this);
        internal LobRockImpact Impact => _impact ??= new LobRockImpact(this);
        internal LobRockPose Pose => _pose ??= new LobRockPose(this);
        internal GameObject RockPrefab => rockPrefab;
        internal Sprite RockSprite => rockSprite;
        internal Color RockColor => rockColor;
        internal float RockScale => rockScale;
        internal int RockSortingOrder => rockSortingOrder;
        internal int RandomRockCount => randomRockCount;
        internal float LaunchInterval => launchInterval;
        internal float TravelDuration => travelDuration;
        internal float ArcHeight => arcHeight;
        internal float LaunchHeight => launchHeight;
        internal float SpinDegrees => spinDegrees;
        internal float RandomLandingRadius => randomLandingRadius;
        internal float RandomLandingMinDistance => randomLandingMinDistance;
        internal float ImpactRadius => impactRadius;
        internal float DamageMultiplier => damageMultiplier;
        internal DamageType DamageType => damageType;
        internal ContactFilter2D WhatIsTarget => whatIsTarget;
        internal string ImpactSoundKey => impactSoundKey;
        internal Sprite[] ImpactSprites => impactSprites;
        internal float ImpactFrameInterval => impactFrameInterval;
        internal float ImpactEffectScale => impactEffectScale;
        internal string ImpactSortingLayerName => impactSortingLayerName;
        internal int ImpactSortingOrder => impactSortingOrder;
        internal string AttackStateName => attackStateName;
        internal string AttackBoolParameter => attackBoolParameter;
        internal float AttackFrameNormalizedTime => attackFrameNormalizedTime;
        internal float AttackPoseDuration => attackPoseDuration;
        internal int AnimatorLayer => animatorLayer;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Circle;
        public override bool UsesOwnerAttackAnimation => false;
        protected override UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            owner.GetCompo<EntityMover>()?.StopImmediately();
            return UniTask.CompletedTask;
        }

        protected override UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
            => Sequence.OnExecutePattern(owner, ct);
        internal DamageData GetDamage(Enemy owner, float multiplier) => GetAttackDamage(owner, multiplier);
    }
}
