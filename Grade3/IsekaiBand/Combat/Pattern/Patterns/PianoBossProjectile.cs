using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    public class PianoBossProjectile : MonoBehaviour
    {
        private DamageData _damage;
        private Entity _owner;
        private LayerMask _targetMask;
        private Vector2 _direction;
        internal DamageData Damage { get => _damage; set => _damage = value; }
        internal Entity Owner { get => _owner; set => _owner = value; }
        internal LayerMask TargetMask { get => _targetMask; set => _targetMask = value; }
        internal Vector2 Direction { get => _direction; set => _direction = value; }
        private PianoProjectileMotion _motion;
        internal PianoProjectileMotion Motion => _motion ??= new PianoProjectileMotion(this);
        private PianoProjectileMovement _movement;
        internal PianoProjectileMovement Movement => _movement ??= new PianoProjectileMovement(this);
        private PianoProjectileVisual _visual;
        internal PianoProjectileVisual Visual => _visual ??= new PianoProjectileVisual(this);
        private PianoProjectileTrail _trail;
        internal PianoProjectileTrail Trail => _trail ??= new PianoProjectileTrail(this);

        public static PianoBossProjectile Create(string objectName, Vector2 position, Sprite sprite, Color color, float scale, int sortingOrder)
            => PianoProjectileVisual.Create(objectName, position, sprite, color, scale, sortingOrder);
        public void EnableTrail(Color color, float time, float startWidth, float endWidth)
            => Trail.EnableTrail(color, time, startWidth, endWidth);
        internal void SetupVisual(Sprite sprite, Color color, int sortingOrder) => Visual.SetupVisual(sprite, color, sortingOrder);
        public void InitBezier(Vector3 start, Vector3 controlA, Vector3 controlB, Vector3 end, DamageData damage,
            float duration, float lifeTime, Entity owner, LayerMask targetMask)
            => Motion.InitBezier(start, controlA, controlB, end, damage, duration, lifeTime, owner, targetMask);
        public void InitDescendingHelix(float centerX, float startY, float endY, float amplitude, float turns,
            float initialPhase, DamageData damage, float duration, float lifeTime, Entity owner, LayerMask targetMask)
            => Motion.InitDescendingHelix(centerX, startY, endY, amplitude, turns, initialPhase, damage, duration, lifeTime, owner, targetMask);
        private void Update() => Movement.Update();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _targetMask.value) == 0)
                return;

            IDamageable damageable = other.GetComponent<IDamageable>() ?? other.GetComponentInParent<IDamageable>();
            if (damageable == null)
                return;

            damageable.TakeDamage(_damage, _direction, _owner);
            ProjectilePool.Push(gameObject);
        }
    }
}
