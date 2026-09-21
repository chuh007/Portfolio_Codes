using _Code.LCH._02.Scripts.Core;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoProjectileMovement
    {
        private readonly PianoBossProjectile _projectile;

        public PianoProjectileMovement(PianoBossProjectile projectile) => _projectile = projectile;

        public void Update()
        {
            _projectile.Motion.Timer += Time.deltaTime;
            if (_projectile.Motion.Timer >= _projectile.Motion.LifeTime)
            {
                ProjectilePool.Push(_projectile.gameObject);
                return;
            }

            if (_projectile.Motion.Kind == PianoProjectileMotion.MotionType.DescendingHelix)
                MoveDescendingHelix();
            else
                MoveBezier();
        }

        public void MoveBezier()
        {
            float t = Mathf.Clamp01(_projectile.Motion.Timer / _projectile.Motion.MoveDuration);
            Vector3 previous = _projectile.transform.position;
            _projectile.transform.position = EvaluateBezier(t);

            Vector2 tangent = _projectile.transform.position - previous;
            if (tangent.sqrMagnitude > 0.0001f)
            {
                _projectile.Direction = tangent.normalized;
                RotateTo(_projectile.Direction);
            }
        }

        public void MoveDescendingHelix()
        {
            float t = Mathf.Clamp01(_projectile.Motion.Timer / _projectile.Motion.MoveDuration);
            Vector3 previous = _projectile.transform.position;
            _projectile.transform.position = EvaluateDescendingHelix(t);

            Vector2 tangent = _projectile.transform.position - previous;
            if (tangent.sqrMagnitude > 0.0001f)
            {
                _projectile.Direction = tangent.normalized;
                RotateTo(_projectile.Direction);
            }
        }

        public Vector3 EvaluateBezier(float t)
        {
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * _projectile.Motion.BezierStart
                   + 3f * oneMinusT * oneMinusT * t * _projectile.Motion.BezierControlA
                   + 3f * oneMinusT * t * t * _projectile.Motion.BezierControlB
                   + t * t * t * _projectile.Motion.BezierEnd;
        }

        public Vector3 EvaluateDescendingHelix(float t)
        {
            float angle = _projectile.Motion.HelixInitialPhase + t * _projectile.Motion.HelixTurns * Mathf.PI * 2f;
            float x = _projectile.Motion.HelixCenterX + Mathf.Sin(angle) * _projectile.Motion.HelixAmplitude;
            float y = Mathf.LerpUnclamped(_projectile.Motion.HelixStartY, _projectile.Motion.HelixEndY, t);
            return new Vector3(x, y, _projectile.transform.position.z);
        }

        public void RotateTo(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.001f)
                return;

            _projectile.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
    }
}
