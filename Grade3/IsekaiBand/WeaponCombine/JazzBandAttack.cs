using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class JazzBandAttack : CombineWeaponBase
    {
        private const int BaseNoteCount = 8;
        private readonly JazzBandStats _stats = new();
        private readonly BandRuntimeObjects _notes = new();
        private readonly JazzBandAreaAttack _areaAttack;
        private readonly JazzBandBassZones _zones;
        private readonly JazzBandNotes _noteAttack;

        public override CombineWeaponType CombinationType => CombineWeaponType.JazzBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Drum;

        internal Vector3 Position => OwnerPosition;

        public JazzBandAttack()
        {
            _areaAttack = new JazzBandAreaAttack(this, DamageEnemy);
            _zones = new JazzBandBassZones(this, _stats, _areaAttack);
            _noteAttack = new JazzBandNotes(this, _stats, _notes, HandleNoteImpact);
        }

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => SetRuntimeStat(AttackStatName.Cooldown, this, _stats.Configure(ingredients));

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _notes.RemoveDestroyed();
            _zones.RemoveDestroyedZones();
        }

        public override void Dispose()
        {
            _notes.ReturnToPool(reverseOrder: true, activeOnly: true);
            _zones.Dispose();
            base.Dispose();
        }

        internal void TickBassZone(Vector3 position) => _zones.TickBassZone(position);

        protected override void OnAttack()
        {
            _areaAttack.SpawnDrumShockwave(
                OwnerPosition,
                _stats.LaunchShockwaveDamage,
                ScaleCommonRange(_stats.LaunchShockwaveRadius),
                _stats.ShockwaveKnockback * 1.35f,
                true);

            int noteCount = ResolveProjectileCount(BaseNoteCount);
            Transform target = ManualTargetingService.FindPriorityOrNearest(
                OwnerPosition,
                ScaleCommonRange(_stats.NoteRange));
            if (target != null)
                _noteAttack.FireTargetedFan(target, noteCount);
            else
                _noteAttack.FireRadialNotes(noteCount);

            BandRuntimeVisuals.SpawnSignatureBurst(
                OwnerPosition,
                ScaleCommonRange(_stats.LaunchShockwaveRadius),
                new Color(1f, 0.72f, 0.18f, 0.62f),
                new Color(0.18f, 0.86f, 0.72f, 0.62f),
                noteCount,
                Random.Range(0f, 360f),
                0.36f,
                55);
        }

        private void HandleNoteImpact(KeyboardProjectileImpact impact, bool spawnBassZone)
        {
            _areaAttack.SpawnDrumShockwave(
                impact.Position,
                _stats.ImpactShockwaveDamage,
                ScaleCommonRange(_stats.ImpactShockwaveRadius),
                _stats.ShockwaveKnockback,
                false);

            if (spawnBassZone)
                _zones.SpawnBassZone(impact.Position);
        }
    }
}
