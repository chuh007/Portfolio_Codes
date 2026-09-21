using Chuh007Lib.Entities.Entities;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class BandRuntimeVisuals
    {
        public static void SpawnSignatureBurst(
            Vector3 position,
            float radius,
            Color primary,
            Color secondary,
            int rayCount,
            float rotationOffset = 0f,
            float duration = 0.34f,
            int sortingOrder = 56)
        {
            float safeRadius = Mathf.Max(0.1f, radius);
            int safeRayCount = Mathf.Max(2, rayCount);
            BuildVisualEffect.SpawnCircle(
                position,
                safeRadius,
                primary,
                duration,
                sortingOrder,
                true);
            BuildVisualEffect.SpawnCircle(
                position,
                safeRadius * 0.48f,
                secondary,
                duration * 0.72f,
                sortingOrder + 1);

            for (int i = 0; i < safeRayCount; i++)
            {
                float angle = rotationOffset + 360f * i / safeRayCount;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                BuildVisualEffect.SpawnLine(
                    position + (Vector3)(direction * (safeRadius * 0.2f)),
                    position + (Vector3)(direction * safeRadius),
                    i % 2 == 0 ? primary : secondary,
                    Mathf.Max(0.06f, safeRadius * 0.035f),
                    duration,
                    sortingOrder + 1);
            }
        }

        public static void SpawnChord(
            Vector3 position,
            Vector2 direction,
            float length,
            float spread,
            Color color,
            float duration = 0.28f,
            int sortingOrder = 55)
        {
            Vector2 forward = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
            Vector2 perpendicular = new(-forward.y, forward.x);
            for (int i = -1; i <= 1; i++)
            {
                Vector3 offset = (Vector3)(perpendicular * (spread * i));
                BuildVisualEffect.SpawnLine(
                    position + offset,
                    position + offset + (Vector3)(forward * length),
                    color,
                    Mathf.Max(0.07f, spread * 0.24f),
                    duration,
                    sortingOrder + Mathf.Abs(i));
            }
        }

        public static GameObject SpawnProjectile(
            string resourcePath, string objectName, Vector3 position, Quaternion rotation,
            Color color, float scale, int sortingOrder = 51)
            => BandProjectileVisual.SpawnProjectile(
                resourcePath, objectName, position, rotation, color, scale, sortingOrder);

        public static int GetTargetId(Collider2D hit)
        {
            Entity entity = hit != null
                ? hit.GetComponent<Entity>() ?? hit.GetComponentInParent<Entity>()
                : null;
            return entity != null
                ? entity.GetInstanceID()
                : hit != null ? hit.transform.root.GetInstanceID() : 0;
        }

    }
}
