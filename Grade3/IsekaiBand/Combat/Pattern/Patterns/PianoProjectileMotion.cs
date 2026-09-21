using _Code.LCH._02.Scripts.Combat;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoProjectileMotion
    {
        private readonly PianoBossProjectile _projectile;
        public enum MotionType
        {
            Bezier,
            DescendingHelix
        }
        public float LifeTime;
        public float Timer;
        public Vector3 BezierStart;
        public Vector3 BezierControlA;
        public Vector3 BezierControlB;
        public Vector3 BezierEnd;
        public float MoveDuration;
        public MotionType Kind;
        public float HelixCenterX;
        public float HelixStartY;
        public float HelixEndY;
        public float HelixAmplitude;
        public float HelixTurns;
        public float HelixInitialPhase;
        public PianoProjectileMotion(PianoBossProjectile projectile) => _projectile = projectile;

        public void InitBezier(
            Vector3 start,
            Vector3 controlA,
            Vector3 controlB,
            Vector3 end,
            DamageData damage,
            float duration,
            float lifeTime,
            Entity owner,
            LayerMask targetMask)
        {
            _projectile.enabled = true;
            _projectile.Motion.Timer = 0f;
            _projectile.Motion.BezierStart = start;
            _projectile.Motion.BezierControlA = controlA;
            _projectile.Motion.BezierControlB = controlB;
            _projectile.Motion.BezierEnd = end;
            _projectile.Damage = damage;
            _projectile.Motion.MoveDuration = Mathf.Max(0.05f, duration);
            _projectile.Motion.LifeTime = Mathf.Max(_projectile.Motion.MoveDuration, lifeTime);
            _projectile.Owner = owner;
            _projectile.TargetMask = targetMask;
            _projectile.Motion.Kind = MotionType.Bezier;
            _projectile.transform.position = start;

            Vector2 startDirection = controlA - start;
            _projectile.Direction = startDirection.sqrMagnitude > 0.001f ? startDirection.normalized : Vector2.right;
            _projectile.Movement.RotateTo(_projectile.Direction);
        }

        public void InitDescendingHelix(
            float centerX,
            float startY,
            float endY,
            float amplitude,
            float turns,
            float initialPhase,
            DamageData damage,
            float duration,
            float lifeTime,
            Entity owner,
            LayerMask targetMask)
        {
            _projectile.enabled = true;
            _projectile.Motion.Timer = 0f;
            _projectile.Motion.HelixCenterX = centerX;
            _projectile.Motion.HelixStartY = startY;
            _projectile.Motion.HelixEndY = endY;
            _projectile.Motion.HelixAmplitude = Mathf.Max(0f, amplitude);
            _projectile.Motion.HelixTurns = Mathf.Max(0f, turns);
            _projectile.Motion.HelixInitialPhase = initialPhase;
            _projectile.Damage = damage;
            _projectile.Motion.MoveDuration = Mathf.Max(0.05f, duration);
            _projectile.Motion.LifeTime = Mathf.Max(_projectile.Motion.MoveDuration, lifeTime);
            _projectile.Owner = owner;
            _projectile.TargetMask = targetMask;
            _projectile.Motion.Kind = MotionType.DescendingHelix;
            _projectile.transform.position = _projectile.Movement.EvaluateDescendingHelix(0f);

            float angularSpeed = _projectile.Motion.HelixTurns * Mathf.PI * 2f;
            Vector2 startTangent = new Vector2(
                Mathf.Cos(_projectile.Motion.HelixInitialPhase) * angularSpeed * _projectile.Motion.HelixAmplitude,
                _projectile.Motion.HelixEndY - _projectile.Motion.HelixStartY);
            _projectile.Direction = startTangent.sqrMagnitude > 0.001f
                ? startTangent.normalized
                : Vector2.down;
            _projectile.Movement.RotateTo(_projectile.Direction);
        }
    }
}
