using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    public sealed class PianoBossHumanSpiralProjectile : MonoBehaviour
    {
        private DamageData _damage;
        private Entity _owner;
        private LayerMask _targetMask;
        private Vector2 _damageDirection;
        internal DamageData Damage { get => _damage; set => _damage = value; }
        internal Entity Owner { get => _owner; set => _owner = value; }
        internal LayerMask TargetMask { get => _targetMask; set => _targetMask = value; }
        internal Vector2 Direction { get => _damageDirection; set => _damageDirection = value; }
        private HumanSpiralMotion _motion;
        internal HumanSpiralMotion Motion => _motion ??= new HumanSpiralMotion(this);
        private HumanSpiralVisual _visual;
        internal HumanSpiralVisual Visual => _visual ??= new HumanSpiralVisual(this);
        private HumanSpiralTrail _trail;
        internal HumanSpiralTrail Trail => _trail ??= new HumanSpiralTrail(this);

        public static PianoBossHumanSpiralProjectile Create(string objectName, Vector2 position, Sprite sprite, Color color, float scale, int sortingOrder)
            => HumanSpiralVisual.Create(objectName, position, sprite, color, scale, sortingOrder);
        public void EnableTrail(Color color, float time, float startWidth, float endWidth)
            => Trail.EnableTrail(color, time, startWidth, endWidth);
        internal void SetupVisual(Sprite sprite, Color color, int sortingOrder) => Visual.SetupVisual(sprite, color, sortingOrder);
        internal void SetupPhysics() => Visual.SetupPhysics();
        public void Initialize(Vector2 center, float initialAngleDegrees, float initialRadius, float radialSpeed,
            float angularSpeedDegrees, float pathAcceleration, DamageData damage, float lifeTime, Entity owner, LayerMask targetMask)
            => Motion.Initialize(center, initialAngleDegrees, initialRadius, radialSpeed, angularSpeedDegrees, pathAcceleration, damage, lifeTime, owner, targetMask);
        private void Update() => Motion.Update();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _targetMask.value) == 0)
                return;

            IDamageable damageable = other.GetComponent<IDamageable>() ?? other.GetComponentInParent<IDamageable>();
            if (damageable == null)
                return;

            damageable.TakeDamage(_damage, _damageDirection, _owner);
            ProjectilePool.Push(gameObject);
        }
    }
}
