using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.WeaponCombine.FullBand;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class FullBandAttack : VenueBandAttackBase
    {
        private readonly FullBandStats _stats = new();
        private readonly FullBandAreaAttack _areaAttack;
        private readonly FullBandProjectiles _projectiles;
        private readonly FullBandLightning _lightning;
        private readonly FullBandStageVisual _stageVisual;
        private float _vocalTimer;
        private float _rhythmTimer;
        private float _pianoTimer;
        private float _drumTimer;

        public override CombineWeaponType CombinationType => CombineWeaponType.FullBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Vocal;
        protected override bool IsVenuePermanent => true;
        protected override Color VenuePrimaryColor => new(0.98f, 0.18f, 0.72f, 0.25f);
        protected override Color VenueSecondaryColor => new(0.16f, 0.9f, 1f, 0.2f);
        protected override int VenueOpeningRayCount => 20;
        protected override float VenueOpeningRotation => 9f;
        internal Vector3 Position => OwnerPosition;
        internal float VenueRange => VenueRadius;

        public FullBandAttack()
        {
            _areaAttack = new FullBandAreaAttack(this, _stats, DamageEnemy);
            _projectiles = new FullBandProjectiles(this, _stats);
            _lightning = new FullBandLightning(this, _stats, _areaAttack);
            _stageVisual = new FullBandStageVisual(this);
        }

        protected override void ConfigureBandIngredients(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            _stats.Configure(ingredients, BaseVenueRadius, VenueDamagePerSecond, VenueSlowMultiplier);
            BaseVenueRadius = _stats.VenueRadius;
            VenueDamagePerSecond = _stats.VenueDamagePerSecond;
            VenueSlowMultiplier = _stats.VenueSlowMultiplier;
        }

        protected override void OnVenueStarted()
        {
            _stats.SetVenueRadius(BaseVenueRadius);
            _vocalTimer = 0f;
            _rhythmTimer = 0f;
            _pianoTimer = 0f;
            _drumTimer = 0f;
            _lightning.Reset();
            _stageVisual.Begin();

            _areaAttack.EmitVocalPulse(_stageVisual.Elapsed);
            _areaAttack.EmitRhythmPulse();
            _projectiles.FirePianoVolley(_stageVisual.PianoPosition);
            _lightning.Tick(0f);
            _projectiles.FireDrumVolley();
        }

        protected override void TickInsideVenue(float deltaTime)
        {
            _stageVisual.Tick(deltaTime);
            RunAtInterval(ref _vocalTimer, deltaTime, ScaleCommonInterval(0.42f),
                () => _areaAttack.EmitVocalPulse(_stageVisual.Elapsed));
            RunAtInterval(ref _rhythmTimer, deltaTime, ScaleCommonInterval(1.05f), _areaAttack.EmitRhythmPulse);
            RunAtInterval(ref _pianoTimer, deltaTime, ScaleCommonInterval(0.12f),
                () => _projectiles.FirePianoVolley(_stageVisual.PianoPosition));
            RunAtInterval(ref _drumTimer, deltaTime, ScaleCommonInterval(0.78f), _projectiles.FireDrumVolley);
            _lightning.Tick(deltaTime);
        }

        public override void Dispose()
        {
            _projectiles.Dispose();
            _stageVisual.Dispose();
            base.Dispose();
        }

        protected override void RemoveDestroyedRuntimeObjects()
            => _projectiles.RemoveReturnedProjectiles();

        internal void TriggerDrumCollision(Vector3 position) => _areaAttack.TriggerDrumCollision(position);

        internal void OnDrumProjectileReturned(FullBandDrumProjectile projectile)
        {
            if (projectile != null) _projectiles.Remove(projectile.gameObject);
        }

        private static void RunAtInterval(ref float timer, float deltaTime, float interval, System.Action action)
        {
            timer += deltaTime;
            while (timer >= interval)
            {
                timer -= interval;
                action();
            }
        }
    }
}
