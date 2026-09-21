using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal sealed class SkeletonMageFallingSlowFieldContext : MonoBehaviour
    {
        private bool _hasLandingPosition;
        private Vector2 _landingPosition;

        public void Store(Vector2 landingPosition)
        {
            _landingPosition = landingPosition;
            _hasLandingPosition = true;
        }

        public bool TryConsume(out Vector2 landingPosition)
        {
            landingPosition = _landingPosition;
            if (!_hasLandingPosition)
                return false;

            _hasLandingPosition = false;
            return true;
        }

        public void Clear()
        {
            _hasLandingPosition = false;
        }

        public static Vector2 GetLandingPosition(Enemy owner)
        {
            if (owner != null && owner.target != null)
                return owner.target.transform.position;

            return owner != null ? owner.transform.position : Vector2.zero;
        }

        public static void StorePreparedLandingPosition(Enemy owner, Vector2 landingPosition)
        {
            if (owner == null)
                return;

            GetOrAddContext(owner).Store(landingPosition);
        }

        public static bool TryConsumePreparedLandingPosition(Enemy owner, out Vector2 landingPosition)
        {
            landingPosition = default;
            if (owner == null || !owner.TryGetComponent(out SkeletonMageFallingSlowFieldContext context))
                return false;

            return context.TryConsume(out landingPosition);
        }

        public static void ClearPreparedLandingPosition(Enemy owner)
        {
            if (owner == null || !owner.TryGetComponent(out SkeletonMageFallingSlowFieldContext context))
                return;

            context.Clear();
        }

        public static SkeletonMageFallingSlowFieldContext GetOrAddContext(Enemy owner)
        {
            if (owner.TryGetComponent(out SkeletonMageFallingSlowFieldContext context))
                return context;

            context = owner.gameObject.AddComponent<SkeletonMageFallingSlowFieldContext>();
            context.hideFlags = HideFlags.HideInInspector;
            return context;
        }
    }
}
