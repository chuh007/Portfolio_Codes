using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Visual;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class HumanSpiralVisual
    {
        private readonly PianoBossHumanSpiralProjectile _projectile;
        private SpriteRenderer _renderer;
        public SpriteRenderer Renderer => _renderer;
        public HumanSpiralVisual(PianoBossHumanSpiralProjectile projectile) => _projectile = projectile;

        public static PianoBossHumanSpiralProjectile Create(
            string objectName,
            Vector2 position,
            Sprite sprite,
            Color color,
            float scale,
            int sortingOrder)
        {
            GameObject projectileObject = ProjectilePool.Pop(
                "PianoHumanSpiralProjectile",
                position,
                Quaternion.identity);
            if (projectileObject == null)
                return null;

            projectileObject.name = objectName;
            projectileObject.transform.localScale = Vector3.one * Mathf.Max(0.05f, scale);

            int projectileLayer = LayerMask.NameToLayer("EnemyProjectile");
            if (projectileLayer >= 0)
                projectileObject.layer = projectileLayer;

            PianoBossHumanSpiralProjectile projectile =
                ProjectilePool.GetOrAddComponent<PianoBossHumanSpiralProjectile>(projectileObject);
            if (projectile == null)
            {
                ProjectilePool.Push(projectileObject);
                return null;
            }

            projectile.SetupVisual(sprite, color, sortingOrder);
            projectile.SetupPhysics();
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
        }

        public void SetupPhysics()
        {
            CircleCollider2D hitbox = ProjectilePool.GetOrAddComponent<CircleCollider2D>(_projectile.gameObject);
            hitbox.enabled = true;
            hitbox.isTrigger = true;
            hitbox.radius = 0.28f;

            Rigidbody2D body = ProjectilePool.GetOrAddComponent<Rigidbody2D>(_projectile.gameObject);
            body.simulated = true;
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
        }
    }
}
