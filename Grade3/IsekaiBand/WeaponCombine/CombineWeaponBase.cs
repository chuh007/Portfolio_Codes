using _Code.LCH._02.Scripts.Player.Attack;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    /// <summary>
    /// 기존의 웨폰 구조는 도무지 사용이 어려워서 새로이 만듬.
    /// 팩토리 메서드 패턴을 사용한다. CombineWeaponBase가 제품 기반.
    /// </summary>
    public abstract class CombineWeaponBase : PlayerAttackBase
    {
        public abstract CombineWeaponType CombinationType { get; }
        public abstract WeaponType PrimaryWeaponType { get; }
        internal CombineWeaponAudio AttackAudio { get; } = new();
        internal int CountProjectiles(int baseCount) => ResolveProjectileCount(baseCount);

        public virtual void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
        {
        }

        public override void Dispose()
        {
            AttackAudio.StopVocal();
            base.Dispose();
        }

        protected float TuneDamage(float value, int ingredientCount)
            => CombineWeaponTuning.Damage(value, CombinationType, ingredientCount);

        protected void DamageEnemy(Collider2D hit, Vector3 source, float damage, float range, float knockback = 0f)
        {
            if (!IsTargetCollider(hit) || !hit.CompareTag("Enemy")) return;

            damage = ScaleCommonDamage(damage);

            if (hit.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(new DamageData(damage, false));

            Vector2 direction = ((Vector2)hit.transform.position - (Vector2)source).normalized;
            if (knockback > 0f && hit.TryGetComponent<IKnockbackable>(out var knockbackable))
                knockbackable.ApplyKnockback(direction, knockback);

            var context = new AttackEventContext(
                this, hit, hit.transform.position, direction, damage, range, range,
                0f, 0f, 0, 0f, knockback);
            RaiseHit(context);

            if (hit.TryGetComponent<Entity>(out var entity) && entity.IsDead)
                RaiseKill(context);
        }
    }
}
