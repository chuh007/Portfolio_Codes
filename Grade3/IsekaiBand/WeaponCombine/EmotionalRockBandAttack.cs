using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;
using _Work.CHUH.Code.WeaponCombine.EmotionalRock;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class EmotionalRockBandAttack : VenueBandAttackBase
    {
        private const float PushWaveInterval = 0.42f;
        private const float StartBarrageInterval = 0.055f;
        private const float EndBarrageInterval = 0.42f;
        private const float BarrageCycleDuration = 7f;
        private readonly EmotionalRockStats _stats = new();
        private readonly BandRuntimeObjects _projectiles = new();
        private readonly EmotionalRockWaves _waves;
        private readonly EmotionalRockBarrage _barrage;
        private float _pushTimer;
        private float _barrageTimer = StartBarrageInterval;
        private float _barrageCycleElapsed;

        public override CombineWeaponType CombinationType => CombineWeaponType.EmotionalRockBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Vocal;
        protected override Color VenuePrimaryColor => new(0.86f, 0.16f, 0.92f, 0.22f);
        protected override Color VenueSecondaryColor => new(1f, 0.48f, 0.18f, 0.16f);
        protected override int VenueOpeningRayCount => 10;
        protected override float VenueOpeningRotation => 18f;

        internal Vector3 Position => OwnerPosition;

        public EmotionalRockBandAttack()
        {
            _waves = new EmotionalRockWaves(this, _stats, DamageEnemy);
            _barrage = new EmotionalRockBarrage(this, _stats, _waves, _projectiles);
        }

        protected override void ConfigureBandIngredients(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients) => _stats.Configure(ingredients);

        public override void Dispose()
        {
            _projectiles.ReturnToPool();
            base.Dispose();
        }

        protected override void TickPersistentBandEffect(float deltaTime)
        {
            _barrage.InitializeInstrument();

            _barrageCycleElapsed = Mathf.Repeat(
                _barrageCycleElapsed + deltaTime,
                BarrageCycleDuration);
            _barrageTimer += deltaTime;
            float interval = Mathf.Lerp(
                StartBarrageInterval,
                EndBarrageInterval,
                Mathf.Pow(_barrageCycleElapsed / BarrageCycleDuration, 2f));
            interval = ScaleCommonInterval(interval);

            while (_barrageTimer >= interval)
            {
                _barrageTimer -= interval;
                _barrage.FireNextInstrument();
            }

            _pushTimer += deltaTime;
            float pushInterval = ScaleCommonInterval(PushWaveInterval);
            while (_pushTimer >= pushInterval)
            {
                _pushTimer -= pushInterval;
                _waves.FirePushWave(PushWaveInterval);
            }
        }

        protected override void RemoveDestroyedRuntimeObjects() => _projectiles.RemoveDestroyed();

        internal void HitProjectile(
            EmotionalRockProjectile projectile,
            Collider2D hit,
            Vector3 position,
            float damage,
            float radius,
            bool applySlow)
        {
            DamageEnemy(hit, position, damage, radius);
            if (applySlow)
            {
                var slowable = hit.GetComponent<ISlowable>() ?? hit.GetComponentInParent<ISlowable>();
                slowable?.ApplySlow(0.7f, ScaleCommonSlowDuration(0.5f));
            }
        }

        internal void ProjectileEnded(EmotionalRockProjectile projectile)
        {
            if (projectile != null)
                _projectiles.Remove(projectile.gameObject);
        }
    }
}
