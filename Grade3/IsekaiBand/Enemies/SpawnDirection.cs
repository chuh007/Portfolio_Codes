using _Work.CHUH.Code.StageSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies
{
    internal static class SpawnDirection
    {
        public static Vector2 GetPlayerForwardDirection(SpawnerPositionContext context)
        {
            if (context.Player == null)
                return Vector2.right;

            if (context.PlayerMovement != null && context.PlayerMovement.LastMoveDirection.sqrMagnitude > 0.0001f)
                return context.PlayerMovement.LastMoveDirection.normalized;

            if (context.Player.TryGetComponent(out Rigidbody2D rb) && rb.linearVelocity.sqrMagnitude > 0.0001f)
                return rb.linearVelocity.normalized;

            return Vector2.right;
        }

        public static Vector2 RandomDirection()
        {
            Vector2 direction = Random.insideUnitCircle;
            if (direction.sqrMagnitude <= 0.0001f)
                return Vector2.right;

            return direction.normalized;
        }

        public static Vector2 RandomDirectionInCone(Vector2 forward, float angle)
        {
            if (forward.sqrMagnitude <= 0.0001f)
                forward = Vector2.right;

            float halfAngle = Mathf.Clamp(angle, 0f, 180f) * 0.5f;
            float randomAngle = Random.Range(-halfAngle, halfAngle);
            return Quaternion.Euler(0f, 0f, randomAngle) * forward.normalized;
        }
    }
}
