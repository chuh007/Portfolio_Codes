using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal sealed class PianoBossHumanClosingRingContext : MonoBehaviour
    {
        private Vector2 _center;
        private int _openDirection;
        private bool _hasPlan;

        public void Store(Vector2 center, int openDirection)
        {
            _center = center;
            _openDirection = openDirection;
            _hasPlan = true;
        }

        public bool TryConsume(out Vector2 center, out int openDirection)
        {
            center = _center;
            openDirection = _openDirection;
            bool hasPlan = _hasPlan;
            _hasPlan = false;
            return hasPlan;
        }


        public static Vector2 GetTargetCenter(Enemy owner)
        {
            if (owner == null)
                return Vector2.zero;

            return owner.target != null
                ? owner.target.transform.position
                : owner.transform.position;
        }

        public static PianoBossHumanClosingRingContext GetOrAddContext(Enemy owner)
        {
            if (owner.TryGetComponent(out PianoBossHumanClosingRingContext context))
                return context;

            context = owner.gameObject.AddComponent<PianoBossHumanClosingRingContext>();
            context.hideFlags = HideFlags.HideInInspector;
            return context;
        }

        public static bool TryConsumePlan(Enemy owner, out Vector2 center, out int openDirection)
        {
            center = Vector2.zero;
            openDirection = 0;
            return owner != null
                   && owner.TryGetComponent(out PianoBossHumanClosingRingContext context)
                   && context.TryConsume(out center, out openDirection);
        }
    }
}
