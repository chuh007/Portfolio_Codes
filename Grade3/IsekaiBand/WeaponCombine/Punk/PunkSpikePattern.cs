using _Code.LCH._02.Scripts.Combat;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class PunkSpikePattern
    {
        private const int SpikeCount = 7;

        internal static float[] CreateAngles(
            Vector3 origin, float minimumDistance, float range, ContactFilter2D filter, Collider2D[] hits)
        {
            int count = Physics2D.OverlapCircle(origin, range, filter, hits);
            Transform target = FindTarget(origin, minimumDistance, range, hits, count);
            float sector = 360f / SpikeCount;
            float offset = Random.Range(0f, sector);
            if (target != null)
            {
                Vector2 direction = (Vector2)target.position - (Vector2)origin;
                offset = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            }

            var angles = new float[SpikeCount];
            for (int i = 0; i < angles.Length; i++)
            {
                // 첫 가시는 적을 정확히 조준하고, 나머지는 원형 분포 안에서 흔들리게 한다.
                float jitter = target != null && i == 0
                    ? 0f : Random.Range(-sector * 0.35f, sector * 0.35f);
                angles[i] = offset + i * sector + jitter;
            }
            return angles;
        }

        private static Transform FindTarget(
            Vector3 origin, float minimumDistance, float range, Collider2D[] hits, int count)
        {
            Transform nearest = null;
            float nearestDistance = range * range;
            float minimumSqrDistance = minimumDistance * minimumDistance;
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = hits[i];
                if (hit == null || !hit.CompareTag("Enemy") || !ManualTargetingService.IsValid(hit.transform))
                    continue;

                Vector2 direction = (Vector2)hit.transform.position - (Vector2)origin;
                float distance = direction.sqrMagnitude;
                if (distance < minimumSqrDistance || distance > range * range)
                    continue;
                if (ManualTargetingService.IsPriorityTarget(hit.transform))
                    return hit.transform;
                if (distance > nearestDistance)
                    continue;

                nearest = hit.transform;
                nearestDistance = distance;
            }
            return nearest;
        }
    }
}
