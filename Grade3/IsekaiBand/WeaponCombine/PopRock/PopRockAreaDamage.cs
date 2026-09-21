using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class PopRockAreaDamage
    {
        private const float AfterglowSlowMultiplier = 0.78f;
        private const float AfterglowTickInterval = 0.45f;
        private readonly PopRockBandAttack _source;
        private readonly System.Action<Collider2D, Vector3, float, float, float> _damageEnemy;
        private readonly Collider2D[] _hits = new Collider2D[128];
        private readonly HashSet<int> _hitTargetIds = new();

        public PopRockAreaDamage(PopRockBandAttack source,
            System.Action<Collider2D, Vector3, float, float, float> damageEnemy)
        {
            _source = source;
            _damageEnemy = damageEnemy;
        }

        public void Damage(Vector3 position, float radius, float damage, float knockback, bool applySlow)
        {
            _hitTargetIds.Clear();
            int count = Physics2D.OverlapCircle(position, radius, _source.TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy") || !ManualTargetingService.IsValid(hit.transform))
                    continue;

                int targetId = GetTargetId(hit);
                if (!_hitTargetIds.Add(targetId))
                    continue;

                _damageEnemy(hit, position, damage, radius, knockback);
                if (!applySlow) continue;
                ISlowable slowable = hit.GetComponent<ISlowable>()
                                     ?? hit.GetComponentInParent<ISlowable>();
                slowable?.ApplySlow(
                    AfterglowSlowMultiplier,
                    _source.ScaleCommonSlowDuration(AfterglowTickInterval + 0.1f));
            }

        }

        private static int GetTargetId(Collider2D hit)
        {
            Entity entity = hit.GetComponent<Entity>() ?? hit.GetComponentInParent<Entity>();
            return entity != null ? entity.GetInstanceID() : hit.transform.root.GetInstanceID();
        }
    }
}
