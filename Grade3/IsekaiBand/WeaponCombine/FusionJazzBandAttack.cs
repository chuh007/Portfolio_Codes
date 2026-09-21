using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;
using TargetCandidate = _Work.CHUH.Code.WeaponCombine.FusionJazzTargets.TargetCandidate;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class FusionJazzBandAttack : VenueBandAttackBase
    {
        private const string ProjectilePrefabPath = "LCH/RuntimePrefabs/ElectricBoltProjectile";
        private const float MinFireInterval = 0.04f;
        private const float MaxFireInterval = 0.65f;
        private const float TargetRefreshInterval = 0.06f;
        private readonly FusionJazzStats _stats = new();
        private readonly FusionJazzTargets _targets;
        private readonly Dictionary<int, float> _nextFireTimes = new();
        private readonly BandRuntimeObjects _projectiles = new();
        private float _refreshTimer = TargetRefreshInterval;
        private int _visualShotIndex;

        public override CombineWeaponType CombinationType => CombineWeaponType.FusionJazzBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Guitar;
        protected override Color VenuePrimaryColor => new(0.1f, 0.78f, 1f, 0.22f);
        protected override Color VenueSecondaryColor => new(1f, 0.72f, 0.12f, 0.16f);
        protected override int VenueOpeningRayCount => 14;
        protected override float VenueOpeningRotation => 6f;

        internal Vector3 Position => OwnerPosition;

        public FusionJazzBandAttack() => _targets = new FusionJazzTargets(this, _stats);

        protected override void ConfigureBandIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => _stats.Configure(ingredients, VenueRadius);

        public override void Dispose()
        {
            _projectiles.ReturnToPool();
            _nextFireTimes.Clear();
            _targets.Clear();
            base.Dispose();
        }

        protected override void TickPersistentBandEffect(float deltaTime)
        {
            _refreshTimer += deltaTime;
            if (_refreshTimer >= TargetRefreshInterval)
            {
                _refreshTimer -= TargetRefreshInterval;
                _targets.Refresh(_nextFireTimes);
            }

            float now = Time.time;
            for (int i = 0; i < _targets.Candidates.Count; i++)
            {
                TargetCandidate candidate = _targets.Candidates[i];
                if (candidate.Target == null || !ManualTargetingService.IsValid(candidate.Target))
                    continue;

                if (_nextFireTimes.TryGetValue(candidate.Id, out float nextTime) && now < nextTime)
                    continue;

                float normalizedDistance = Mathf.Clamp01(candidate.Distance / Mathf.Max(0.1f, ScaleCommonRange(_stats.TargetRange)));
                float interval = ScaleCommonInterval(Mathf.Lerp(MinFireInterval, MaxFireInterval, normalizedDistance));
                _nextFireTimes[candidate.Id] = now + interval;
                int projectileCount = CountProjectiles(1);
                for (int shot = 0; shot < projectileCount; shot++)
                    FireAt(candidate, (shot - (projectileCount - 1) * 0.5f) * 0.18f);
            }
        }

        protected override void RemoveDestroyedRuntimeObjects() => _projectiles.RemoveDestroyed();

        internal void HitTarget(
            FusionJazzProjectile projectile,
            Collider2D target,
            Vector3 position)
        {
            if (target != null && target.CompareTag("Enemy"))
                DamageEnemy(target, position, _stats.ProjectileDamage, 0.4f);

        }

        internal void ProjectileEnded(FusionJazzProjectile projectile)
        {
            if (projectile != null)
                _projectiles.Remove(projectile.gameObject);
        }

        private void FireAt(TargetCandidate candidate, float lateralOffset)
        {
            Vector2 direction = ((Vector2)candidate.Target.position - (Vector2)OwnerPosition).normalized;
            Vector3 origin = OwnerPosition + (Vector3)(new Vector2(-direction.y, direction.x) * lateralOffset);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                ProjectilePrefabPath,
                "FusionJazzTargetProjectile",
                origin,
                Quaternion.Euler(0f, 0f, angle),
                _visualShotIndex++ % 2 == 0
                    ? new Color(0.3f, 0.9f, 1f, 1f)
                    : new Color(1f, 0.72f, 0.16f, 1f),
                1.2f,
                54);
            if (obj == null)
                return;

            (obj.GetComponent<FusionJazzProjectile>() ?? obj.AddComponent<FusionJazzProjectile>()).Init(
                this,
                candidate.Target,
                candidate.Collider,
                _stats.ProjectileSpeed,
                2.2f);
            _projectiles.Add(obj);
        }
    }
}
