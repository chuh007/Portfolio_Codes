using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class JazzPopBandAttack : VenueBandAttackBase
    {
        private const float VolleyInterval = 2.8f;
        private const float TravelDamageMultiplier = 0.25f;
        private const float TravelHitRadius = 0.65f;
        private readonly JazzPopStats _stats = new();
        private readonly JazzPopVolley _volley;
        private readonly BandRuntimeObjects _projectiles = new();
        private readonly Collider2D[] _hits = new Collider2D[128];
        private readonly HashSet<int> _hitIds = new();
        private float _volleyTimer = VolleyInterval;

        public override CombineWeaponType CombinationType => CombineWeaponType.JazzPopBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Keyboard;
        protected override Color VenuePrimaryColor => new(1f, 0.24f, 0.62f, 0.22f);
        protected override Color VenueSecondaryColor => new(0.26f, 0.9f, 1f, 0.16f);
        protected override int VenueOpeningRayCount => 12;
        protected override float VenueOpeningRotation => 15f;

        internal Vector3 Position => OwnerPosition;

        public JazzPopBandAttack() => _volley = new JazzPopVolley(this, _stats, _projectiles);

        protected override void ConfigureBandIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => _stats.Configure(ingredients, VenueRadius, ApplyPermanentProjectileSpeedToDuration);

        public override void Dispose()
        {
            _projectiles.ReturnToPool();
            base.Dispose();
        }

        protected override void TickPersistentBandEffect(float deltaTime)
        {
            _volleyTimer += deltaTime;
            float interval = ScaleCommonInterval(VolleyInterval);
            while (_volleyTimer >= interval)
            {
                _volleyTimer -= interval;
                _volley.FireSpringVolley();
            }
        }

        protected override void RemoveDestroyedRuntimeObjects() => _projectiles.RemoveDestroyed();

        internal void Explode(JazzPopSpringProjectile projectile, Vector3 position)
        {
            _hitIds.Clear();
            int count = Physics2D.OverlapCircle(position, ScaleCommonRange(_stats.ExplosionRadius), TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (_hitIds.Add(targetId))
                    DamageEnemy(hit, position, _stats.ExplosionDamage, ScaleCommonRange(_stats.ExplosionRadius));
            }

            BuildVisualEffect.SpawnCircle(
                position,
                ScaleCommonRange(_stats.ExplosionRadius),
                new Color(1f, 0.24f, 0.62f, 0.8f),
                0.58f,
                57,
                true);
            BuildVisualEffect.SpawnCircle(
                position,
                ScaleCommonRange(_stats.ExplosionRadius) * 0.52f,
                new Color(0.3f, 0.96f, 1f, 0.76f),
                0.36f,
                58,
                true);

            if (projectile != null)
                _projectiles.Remove(projectile.gameObject);
        }

        internal void DamagePassingTarget(Collider2D hit, Vector3 position)
            => DamageEnemy(
                hit,
                position,
                _stats.ExplosionDamage * TravelDamageMultiplier,
                TravelHitRadius);
    }
}
