using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class RockBandAttack : CombineWeaponBase
    {
        private const string RockPrefabPath = "LCH/RuntimePrefabs/HardRockProjectile";
        private const int RockCount = 16;
        private const float BaseRockVisualScale = 2.25f;
        private const float ProjectileSizeMultiplier = 1.10f;

        private readonly RockStats _stats = new();
        private readonly BandRuntimeObjects _rocks = new();

        public override CombineWeaponType CombinationType => CombineWeaponType.RockBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Guitar;

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => SetRuntimeStat(AttackStatName.Cooldown, this, _stats.Configure(ingredients, ApplyPermanentAttackRange));

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            _rocks.RemoveDestroyed();
        }

        public override void Dispose()
        {
            _rocks.ReturnToPool();
            base.Dispose();
        }

        protected override void OnAttack()
        {
            int rockCount = ResolveProjectileCount(RockCount);
            float startAngle = Random.Range(0f, 360f / rockCount);
            for (int i = 0; i < rockCount; i++)
            {
                float angle = startAngle + 360f * i / rockCount;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                    RockPrefabPath,
                    "RockBandProjectile",
                    OwnerPosition,
                    Quaternion.Euler(0f, 0f, angle),
                    new Color(0.78f, 0.6f, 0.42f, 1f),
                    BaseRockVisualScale * ProjectileSizeMultiplier);
                if (obj == null)
                    continue;

                (obj.GetComponent<RockBandProjectile>() ?? obj.AddComponent<RockBandProjectile>()).Init(
                    this,
                    direction,
                    _stats.Speed,
                    ScaleCommonRange(_stats.TravelDistance),
                    _stats.Damage,
                    ScaleCommonRange(_stats.HitRadius));
                _rocks.Add(obj);
            }

            BandRuntimeVisuals.SpawnSignatureBurst(
                OwnerPosition,
                3.2f,
                new Color(0.92f, 0.24f, 0.12f, 0.62f),
                new Color(1f, 0.72f, 0.2f, 0.68f),
                RockCount,
                startAngle,
                0.36f,
                52);
        }

        internal void DamageRock(Collider2D hit, Vector3 position, float damage, float radius)
        {
            DamageEnemy(hit, position, damage, radius);
        }

        internal void ProjectileEnded(RockBandProjectile projectile)
        {
            if (projectile != null)
                _rocks.Remove(projectile.gameObject);
        }
    }

}
