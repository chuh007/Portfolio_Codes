using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class RockStarDuoAttack : CombineWeaponBase
    {
        private const float AttackCooldown = 10f;
        private const float BaseExecutionRadius = 11f;
        private const float BossDamage = 100f;

        private readonly HashSet<int> _targetIds = new();
        private float _executionRadius = BaseExecutionRadius;

        public override CombineWeaponType CombinationType => CombineWeaponType.RockStarDuo;
        public override WeaponType PrimaryWeaponType => WeaponType.Guitar;

        public override void ConfigureIngredients(
            IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
            foreach (PlayerAttackBase attack in ingredients.Values)
            {
                if (attack == null)
                    continue;
                _executionRadius = Mathf.Max(
                    _executionRadius,
                    attack.GetBuildStat(AttackStatName.Range, _executionRadius) * 1.45f);
            }

            int ingredientCount = ingredients.Count;
            _executionRadius = CombineWeaponTuning.Range(_executionRadius, ingredientCount);
            SetRuntimeStat(
                AttackStatName.Cooldown,
                this,
                CombineWeaponTuning.Interval(AttackCooldown, ingredientCount, 2f));
        }

        protected override void OnAttack()
        {
            float radius = ScaleCommonRange(_executionRadius);
            _targetIds.Clear();
            using var hitLease = UnityEngine.Pool.ListPool<Collider2D>.Get(out var hits);
            CombatOverlapQuery.FillCircle(OwnerPosition, radius, hits, TargetLayerMask);
            foreach (Collider2D hit in hits)
            {
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                Enemy enemy = hit.GetComponentInParent<Enemy>();
                if (enemy == null)
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (!_targetIds.Add(targetId))
                    continue;

                if (hit.TryGetComponent<IDamageable>(out IDamageable damageable)
                    || (damageable = hit.GetComponentInParent<IDamageable>()) != null)
                {
                    DamageData damage = enemy.IsBoss
                        ? DamageData.Fixed(BossDamage)
                        : new DamageData(float.MaxValue, false, true);
                    damageable.TakeDamage(damage);
                    RaiseHit(new AttackEventContext(
                        this,
                        hit,
                        hit.transform.position,
                        ((Vector2)hit.transform.position - (Vector2)OwnerPosition).normalized,
                        damage.Damage,
                        radius,
                        0f,
                        0f,
                        0f,
                        0,
                        0f,
                        0f,
                        damage.IsFixedDamage));
                }
            }

            BuildVisualEffect.SpawnCircle(
                OwnerPosition,
                radius,
                new Color(1f, 0.24f, 0.12f, 0.82f),
                1.1f,
                58,
                true);
            BuildVisualEffect.SpawnCircle(
                OwnerPosition,
                radius * 0.35f,
                new Color(1f, 0.84f, 0.26f, 0.92f),
                0.72f,
                59,
                true);

            const int rayCount = 24;
            for (int i = 0; i < rayCount; i++)
            {
                Vector2 direction = Quaternion.Euler(
                    0f,
                    0f,
                    360f * i / rayCount) * Vector2.right;
                BuildVisualEffect.SpawnLine(
                    OwnerPosition,
                    OwnerPosition + (Vector3)(direction * radius),
                    i % 2 == 0
                        ? new Color(1f, 0.22f, 0.1f, 0.78f)
                        : new Color(1f, 0.78f, 0.22f, 0.76f),
                    i % 2 == 0 ? 0.18f : 0.11f,
                    0.5f,
                    57);
            }
        }

    }
}
