using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Visual;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoProjectileVisual
    {
        private readonly PianoBossProjectile _projectile;
        private SpriteRenderer _renderer;
        private CircleCollider2D _collider;
        private Rigidbody2D _rigidbody;
        public SpriteRenderer Renderer => _renderer;
        public PianoProjectileVisual(PianoBossProjectile projectile) => _projectile = projectile;

        public static PianoBossProjectile Create(
            string objectName,
            Vector2 position,
            Sprite sprite,
            Color color,
            float scale,
            int sortingOrder)
        {
            GameObject projectileObject = ProjectilePool.Pop(
                "PianoBossProjectile",
                position,
                Quaternion.identity);
            if (projectileObject == null)
                return null;

            projectileObject.name = objectName;
            projectileObject.transform.localScale = Vector3.one * Mathf.Max(0.05f, scale);

            PianoBossProjectile projectile =
                ProjectilePool.GetOrAddComponent<PianoBossProjectile>(projectileObject);
            if (projectile == null)
            {
                ProjectilePool.Push(projectileObject);
                return null;
            }

            projectile.SetupVisual(sprite, color, sortingOrder);
            return projectile;
        }

        public void SetupVisual(Sprite sprite, Color color, int sortingOrder)
        {
            _renderer = ProjectilePool.GetOrAddComponent<SpriteRenderer>(_projectile.gameObject);
            if (_renderer == null)
                return;

            _renderer.sprite = sprite;
            _renderer.color = color;
            BossProjectileRenderLayer.ApplyTo(_renderer, sortingOrder);
            BossProjectileOutline.ApplyTo(_renderer);

            _collider = ProjectilePool.GetOrAddComponent<CircleCollider2D>(_projectile.gameObject);
            _collider.enabled = true;
            _collider.isTrigger = true;
            _collider.radius = 0.28f;

            _rigidbody = ProjectilePool.GetOrAddComponent<Rigidbody2D>(_projectile.gameObject);
            _rigidbody.simulated = true;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.gravityScale = 0f;
        }
    }
}
