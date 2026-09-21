using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public abstract class VenueBandAttackBase : CombineWeaponBase
    {
        private readonly VenuePhase _phase;
        private readonly VenueAreaEffect _areaEffect;
        private VenueGroundVisual _venueGroundVisual;

        protected float BaseVenueRadius { get; set; } = 6f;
        protected float VenueRadius => ScaleCommonRange(BaseVenueRadius);
        protected float VenueDamagePerSecond { get; set; } = 7.7f;
        protected float VenueSlowMultiplier { get; set; } = 0.68f;
        protected virtual bool IsVenuePermanent => false;
        protected virtual Color VenuePrimaryColor => new(0.68f, 0.18f, 0.86f, 0.22f);
        protected virtual Color VenueSecondaryColor => new(0.2f, 0.82f, 1f, 0.14f);
        protected virtual int VenueOpeningRayCount => 8;
        protected virtual float VenueOpeningRotation => 0f;

        protected VenueBandAttackBase()
        {
            _phase = new VenuePhase(BeginVenue, DamageVenue, DrawVenue, TickInsideVenue, EndVenue);
            _areaEffect = new VenueAreaEffect(this, DamageEnemy);
        }

        public sealed override void ConfigureIngredients(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            if (ingredients.TryGetValue(WeaponType.Bass, out PlayerAttackBase bass))
            {
                float bassRange = bass.GetBuildStat(AttackStatName.Range, BaseVenueRadius);
                float bassDamage = bass.GetBuildStat(AttackStatName.Damage, 7f);
                BaseVenueRadius = Mathf.Max(BaseVenueRadius, bassRange * 1.15f);
                VenueDamagePerSecond = Mathf.Max(
                    VenueDamagePerSecond,
                    bass.GetBuildStat(AttackStatName.DotDamagePerSec, bassDamage * 0.4f) * 1.1f);
                VenueSlowMultiplier = Mathf.Clamp01(
                    bass.GetBuildStat(AttackStatName.SlowMultiplier, VenueSlowMultiplier));
            }

            ConfigureBandIngredients(ingredients);
            BaseVenueRadius = CombineWeaponTuning.Range(BaseVenueRadius, ingredients.Count);
            VenueDamagePerSecond = TuneDamage(VenueDamagePerSecond, ingredients.Count);
        }

        public override void Tick(float deltaTime)
        {
            RemoveDestroyedRuntimeObjects();
            TickPersistentBandEffect(deltaTime);
            _phase.Tick(deltaTime, IsVenuePermanent, ScaleCommonInterval(1f));
        }

        public override void Dispose()
        {
            ReleaseVenueGround();
            base.Dispose();
        }

        protected override void OnAttack()
        {
        }

        protected abstract void ConfigureBandIngredients(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients);

        protected virtual void TickPersistentBandEffect(float deltaTime)
        {
        }

        protected virtual void TickInsideVenue(float deltaTime)
        {
        }

        protected virtual void OnVenueStarted()
        {
        }

        protected virtual void OnVenueEnded()
        {
        }

        protected virtual void RemoveDestroyedRuntimeObjects()
        {
        }

        private void BeginVenue()
        {
            ReleaseVenueGround();
            _venueGroundVisual = VenueGroundVisual.Create(
                CombinationType, OwnerTransform, VenueRadius);
            DrawVenue();
            BandRuntimeVisuals.SpawnSignatureBurst(
                OwnerPosition, VenueRadius, VenuePrimaryColor, VenueSecondaryColor,
                VenueOpeningRayCount, VenueOpeningRotation, 0.72f, 49);
            OnVenueStarted();
        }

        private void ReleaseVenueGround()
        {
            if (_venueGroundVisual != null)
                _venueGroundVisual.Release();
            _venueGroundVisual = null;
        }

        private void EndVenue()
        {
            ReleaseVenueGround();
            OnVenueEnded();
        }

        private void DamageVenue()
            => _areaEffect.Damage(OwnerPosition, VenueRadius, VenueDamagePerSecond * 0.2f,
                VenueSlowMultiplier, 0.2f + 0.1f);

        private void DrawVenue()
        {
            _venueGroundVisual?.SetRadius(VenueRadius);
            VenueAreaEffect.Draw(OwnerPosition, VenueRadius, VenuePrimaryColor, VenueSecondaryColor);
        }
    }
}
