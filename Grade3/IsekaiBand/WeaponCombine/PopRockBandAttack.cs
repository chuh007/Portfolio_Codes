using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class PopRockBandAttack : CombineWeaponBase
    {
        private const float ExplosionKnockback = 0.5f;
        private readonly PopRockStats _stats = new();
        private readonly BandRuntimeObjects _projectiles = new();
        private readonly PopRockAreaDamage _area;
        private readonly PopRockAfterglow _afterglow;
        private readonly PopRockScatter _scatter;
        private int _explosionVisualIndex;

        public override CombineWeaponType CombinationType => CombineWeaponType.PopRockBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Guitar;

        internal Vector3 Position => OwnerPosition;

        public PopRockBandAttack()
        {
            _area = new PopRockAreaDamage(this, DamageEnemy);
            _afterglow = new PopRockAfterglow(this, _stats, _area);
            _scatter = new PopRockScatter(this, _stats, _projectiles);
        }

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => SetRuntimeStat(AttackStatName.Cooldown, this,
                _stats.Configure(ingredients, ApplyPermanentAttackRange, ApplyPermanentProjectileSpeedToDuration));

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _projectiles.RemoveDestroyed();
            _afterglow.RemoveDestroyedZones();
        }

        public override void Dispose()
        {
            _projectiles.ReturnToPool(reverseOrder: true);
            _afterglow.Dispose();
            base.Dispose();
        }

        protected override void OnAttack() => _scatter.Fire();

        internal void TickAfterglow(Vector3 position) => _afterglow.TickAfterglow(position);

        internal void Explode(PopRockScatterProjectile projectile, Vector3 position)
        {
            _area.Damage(position, ScaleCommonRange(_stats.ExplosionRadius), _stats.ExplosionDamage, ExplosionKnockback, false);

            BuildVisualEffect.SpawnCircle(
                position, ScaleCommonRange(_stats.ExplosionRadius),
                new Color(1f, 0.16f, 0.58f, 0.72f), 0.38f, 54, true);
            BuildVisualEffect.SpawnCircle(
                position, ScaleCommonRange(_stats.ExplosionRadius) * 0.62f,
                new Color(0.42f, 0.92f, 1f, 0.76f), 0.28f, 55, true);
            if (_explosionVisualIndex++ % 4 == 0)
            {
                BandRuntimeVisuals.SpawnSignatureBurst(
                    position,
                    ScaleCommonRange(_stats.ExplosionRadius) * 1.08f,
                    new Color(1f, 0.16f, 0.58f, 0.62f),
                    new Color(0.35f, 0.92f, 1f, 0.68f),
                    7,
                    12f,
                    0.4f,
                    56);
            }
            RaiseImpact(new AttackEventContext(
                this, null, position, Vector2.zero, _stats.ExplosionDamage,
                ScaleCommonRange(_stats.ExplosionRadius), ScaleCommonRange(_stats.ExplosionRadius), 0f, 0f, 0, 0f, ExplosionKnockback));

            _afterglow.SpawnAfterglowZone(position);
            ProjectileEnded(projectile);
        }

        internal void ProjectileEnded(PopRockScatterProjectile projectile)
        {
            if (projectile != null)
                _projectiles.Remove(projectile.gameObject);
        }
    }
}
