using System.Collections.Generic;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class BalladBandAttack : CombineWeaponBase
    {
        private readonly BalladStats _stats = new();
        private readonly BandRuntimeObjects _notes = new();
        private readonly BalladSpiral _spiral;
        private readonly BalladBassZone _bassZone;

        public override CombineWeaponType CombinationType => CombineWeaponType.BalladBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Keyboard;

        internal Vector3 Position => OwnerPosition;

        public BalladBandAttack()
        {
            _spiral = new BalladSpiral(this, _stats, _notes);
            _bassZone = new BalladBassZone(this, _stats);
        }

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => SetRuntimeStat(AttackStatName.Cooldown, this, _stats.Configure(ingredients));

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _bassZone.Tick(deltaTime);
            _notes.RemoveDestroyed();
        }

        public override void Dispose()
        {
            _notes.ReturnToPool();
            base.Dispose();
        }

        protected override void OnAttack() => _spiral.Fire();

        internal void DamageNote(Collider2D hit, Vector3 position, float damage, float radius)
            => DamageEnemy(hit, position, damage, radius);

        internal void ProjectileEnded(BalladSpiralProjectile projectile)
        {
            if (projectile != null)
                _notes.Remove(projectile.gameObject);
        }
    }
}
