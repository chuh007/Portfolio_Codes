using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal sealed class SlowFieldZone : MonoBehaviour
    {
        private ContactFilter2D _targetFilter;
        private string _moveSpeedStatName;
        private float _radius;
        private float _duration;
        private float _speedMultiplier;
        private float _elapsed;

        private SlowFieldTargets _targets;
        private SlowFieldVisual _visual;
        private SlowFieldTargets Targets => _targets ??= new SlowFieldTargets(this);
        private SlowFieldVisual Visual => _visual ??= new SlowFieldVisual(this);
        internal ContactFilter2D TargetFilter => _targetFilter;
        internal string MoveSpeedStatName => _moveSpeedStatName;
        internal float Radius => _radius;
        internal float Duration => _duration;
        internal float SpeedMultiplier => _speedMultiplier;
        internal float Elapsed => _elapsed;

        public void Initialize(
            Vector2 position,
            float radius,
            float duration,
            float speedMultiplier,
            string moveSpeedStatName,
            ContactFilter2D targetFilter,
            Color visualColor,
            int visualSegments,
            string sortingLayerName,
            int sortingOrder)
        {
            transform.position = position;
            _radius = Mathf.Max(0f, radius);
            _duration = Mathf.Max(0f, duration);
            _speedMultiplier = Mathf.Clamp(speedMultiplier, 0.01f, 1f);
            _moveSpeedStatName = string.IsNullOrWhiteSpace(moveSpeedStatName) ? "moveSpeed" : moveSpeedStatName;
            _targetFilter = targetFilter;

            Visual.CreateVisual(visualColor, visualSegments, sortingLayerName, sortingOrder);
            Targets.RefreshSlowTargets();
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _duration)
            {
                Destroy(gameObject);
                return;
            }

            Targets.RefreshSlowTargets();
            Visual.UpdateVisualAlpha();
        }

        private void OnDestroy()
        {
            _targets?.ClearSlowTargets();
            _visual?.Destroy();
        }
    }
}
