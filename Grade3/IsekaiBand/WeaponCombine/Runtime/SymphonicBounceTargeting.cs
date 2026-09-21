using UnityEngine;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class SymphonicBounceTargeting
    {
        public static Collider2D FindNext(Vector3 position, float range, ContactFilter2D filter,
            Collider2D[] hits, Collider2D current, Collider2D previous)
        {
            int count = Physics2D.OverlapCircle(
                position,
                range,
                filter,
                hits);
            Collider2D best = null;
            Collider2D fallback = null;
            float bestDistance = float.MaxValue;
            float fallbackDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                Collider2D candidate = hits[i];
                if (candidate == current || !IsValidTarget(candidate))
                    continue;

                float distance = ((Vector2)candidate.transform.position
                                  - (Vector2)position).sqrMagnitude;
                if (candidate == previous)
                {
                    if (distance < fallbackDistance)
                    {
                        fallbackDistance = distance;
                        fallback = candidate;
                    }
                    continue;
                }

                if (distance >= bestDistance)
                    continue;
                bestDistance = distance;
                best = candidate;
            }

            return best != null ? best : fallback;
        }

        public static bool IsValidTarget(Collider2D target)
            => target != null
               && target.CompareTag("Enemy")
               && ManualTargetingService.IsValid(target.transform);
    }
}
