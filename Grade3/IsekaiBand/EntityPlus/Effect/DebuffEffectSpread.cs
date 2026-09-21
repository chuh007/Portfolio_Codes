using _Code.LCH._02.Scripts.Card.Build;
using _Code.LCH._02.Scripts.Combat;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    internal static class DebuffEffectSpread
    {
        internal static void Spread(DebuffEffect effect)
        {
            if (!effect.CanSpread || !CommonBuildDebuffApplier.ShouldSpread(effect.SourceBuilds, effect.EffectTarget))
                return;

            float radius = CommonBuildDebuffApplier.GetSpreadRadius(effect.SourceBuilds);
            using var hitLease = UnityEngine.Pool.ListPool<Collider2D>.Get(out var hits);
            using var targetLease = UnityEngine.Pool.ListPool<Entity>.Get(out var targets);
            CombatOverlapQuery.FillCircle(effect.EffectTarget.transform.position, radius, hits);
            foreach (Collider2D hit in hits)
            {
                if (hit == null || !hit.CompareTag("Enemy")) continue;
                Entity target = hit.GetComponent<Entity>() ?? hit.GetComponentInParent<Entity>();
                if (target == null || target == effect.EffectTarget || target.IsDead || targets.Contains(target)) continue;

                targets.Add(target);
                target.GetCompo<EntityEffectController>()?.AddEffect(effect.CreateSpreadEffect(target));
            }
        }
    }
}
