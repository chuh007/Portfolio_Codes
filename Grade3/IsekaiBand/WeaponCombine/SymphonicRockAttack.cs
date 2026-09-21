using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class SymphonicRockAttack : CombineWeaponBase
    {
        private const int MaxActiveProjectiles = 2;
        private const int ProjectilesPerVolley = 2;
        private const float VolleySpreadAngle = 12f;
        private readonly SymphonicRockStats _stats = new();
        private readonly BandRuntimeObjects _projectiles = new();
        private readonly SymphonicRockProjectiles _projectileAttack;
        private readonly SymphonicChainLightning _chain;
        private int _hitVisualIndex;

        public override CombineWeaponType CombinationType => CombineWeaponType.SymphonicRock;
        public override WeaponType PrimaryWeaponType => WeaponType.Keyboard;

        internal Vector3 Position => OwnerPosition;

        public SymphonicRockAttack()
        {
            _projectileAttack = new SymphonicRockProjectiles(this, _stats, _projectiles);
            _chain = new SymphonicChainLightning(this, _stats, DamageEnemy);
        }

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => _stats.Configure(ingredients, this);

        public override void Dispose()
        {
            _projectiles.ReturnToPool();
            base.Dispose();
        }

        protected override void OnAttack()
        {
            _projectiles.RemoveDestroyed();
            int availableSlots = ResolveProjectileCount(MaxActiveProjectiles) - _projectiles.Count;
            if (availableSlots <= 0)
                return;

            Transform target = ManualTargetingService.FindPriorityOrNearest(
                OwnerPosition,
                ScaleCommonRange(_stats.MaxOwnerDistance));
            if (target == null)
                return;

            Vector2 targetDirection = ((Vector2)target.position - (Vector2)OwnerPosition).normalized;
            int projectileCount = Mathf.Min(ResolveProjectileCount(ProjectilesPerVolley), availableSlots);
            for (int i = 0; i < projectileCount; i++)
            {
                float normalized = projectileCount > 1 ? i / (float)(projectileCount - 1) : 0.5f;
                float angleOffset = Mathf.Lerp(
                    -VolleySpreadAngle * 0.5f,
                    VolleySpreadAngle * 0.5f,
                    normalized);
                Vector2 direction = Quaternion.Euler(0f, 0f, angleOffset) * targetDirection;
                _projectileAttack.SpawnBounceProjectile(direction);
            }

            BandRuntimeVisuals.SpawnChord(
                OwnerPosition,
                targetDirection,
                2.4f,
                0.2f,
                new Color(0.38f, 0.66f, 1f, 0.76f),
                0.24f,
                52);
        }

        internal void HitEnemy(Collider2D hit, Vector3 source)
        {
            DamageEnemy(hit, source, _stats.Damage, ScaleCommonRange(_stats.MaxOwnerDistance));
            _chain.TriggerChainLightning(hit);
            if (_hitVisualIndex++ % 3 == 0)
            {
                BandRuntimeVisuals.SpawnSignatureBurst(
                    source,
                    0.85f,
                    new Color(0.34f, 0.66f, 1f, 0.72f),
                    new Color(1f, 0.84f, 0.3f, 0.7f),
                    5,
                    18f,
                    0.2f,
                    54);
            }
        }

        internal void ProjectileEnded(SymphonicRockBounceProjectile projectile)
        {
            if (projectile != null)
                _projectiles.Remove(projectile.gameObject);
        }
    }
}
