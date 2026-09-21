using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class HumanSpiralTrail
    {
        private readonly PianoBossHumanSpiralProjectile _projectile;
        private TrailRenderer _trail;
        private static Material s_TrailMaterial;
        public HumanSpiralTrail(PianoBossHumanSpiralProjectile projectile) => _projectile = projectile;

        public void EnableTrail(Color color, float time, float startWidth, float endWidth)
        {
            _trail = ProjectilePool.GetOrAddComponent<TrailRenderer>(_projectile.gameObject);
            _trail.Clear();
            _trail.time = Mathf.Max(0.05f, time);
            _trail.startWidth = Mathf.Max(0.01f, startWidth);
            _trail.endWidth = Mathf.Max(0f, endWidth);
            _trail.numCornerVertices = 4;
            _trail.numCapVertices = 3;
            _trail.alignment = LineAlignment.View;
            _trail.textureMode = LineTextureMode.Stretch;
            BossProjectileRenderLayer.ApplyTo(
                _trail,
                _projectile.Visual.Renderer != null ? _projectile.Visual.Renderer.sortingOrder - 1 : 0);

            Material material = GetTrailMaterial();
            if (material != null)
                _trail.material = material;

            Color transparent = color;
            transparent.a = 0f;
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(color, 0f),
                    new GradientColorKey(transparent, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(color.a, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            _trail.colorGradient = gradient;
        }

        public static Material GetTrailMaterial()
        {
            if (s_TrailMaterial != null)
                return s_TrailMaterial;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
                return null;

            s_TrailMaterial = new Material(shader)
            {
                hideFlags = HideFlags.DontSave
            };
            return s_TrailMaterial;
        }
    }
}
