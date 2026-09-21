using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class PopRockScatter
    {
        private const int ProjectileCount = 16;
        private const float ExplosionDelay = 0.5f;
        private static readonly float[] TravelDistanceMultipliers = { 0.58f, 0.8f, 1f };
        private readonly PopRockBandAttack _source;
        private readonly PopRockStats _stats;
        private readonly BandRuntimeObjects _projectiles;
        private static Sprite _fallbackSprite;

        public PopRockScatter(PopRockBandAttack source, PopRockStats stats, BandRuntimeObjects projectiles)
        {
            _source = source;
            _stats = stats;
            _projectiles = projectiles;
        }

        public void Fire()
        {
            int projectileCount = _source.CountProjectiles(ProjectileCount);
            float sectorSize = 360f / projectileCount;
            float startAngle = Random.Range(0f, sectorSize);

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + i * sectorSize + Random.Range(-sectorSize * 0.45f, sectorSize * 0.45f);
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                float distanceMultiplier = TravelDistanceMultipliers[i % TravelDistanceMultipliers.Length];
                SpawnScatterProjectile(direction, angle, distanceMultiplier);
            }
        }

        private void SpawnScatterProjectile(
            Vector2 direction,
            float angle,
            float distanceMultiplier)
        {
            GameObject obj = ProjectilePool.Pop(
                "PopRockScatterProjectile",
                _source.Position,
                Quaternion.Euler(0f, 0f, angle));
            if (obj == null)
                return;

            obj.name = "PopRockScatterProjectile";
            obj.transform.SetPositionAndRotation(_source.Position, Quaternion.Euler(0f, 0f, angle));
            obj.transform.localScale = Vector3.one * 1.35f;

            SpriteRenderer renderer = obj.GetComponentInChildren<SpriteRenderer>();
            if (renderer == null)
                renderer = obj.AddComponent<SpriteRenderer>();
            if (renderer.sprite == null)
                renderer.sprite = FallbackSprite;
            renderer.color = new Color(1f, 0.32f, 0.78f, 1f);
            ProjectileRenderLayer.ApplyTo(renderer, 51);
            ProjectileRenderLayer.ApplyTo(obj);

            var runtime = obj.GetComponent<PopRockScatterProjectile>() ?? obj.AddComponent<PopRockScatterProjectile>();
            float clampedDistanceMultiplier = Mathf.Clamp01(distanceMultiplier);
            runtime.Init(
                _source,
                direction,
                _source.ScaleCommonRange(_stats.TravelDistance) * clampedDistanceMultiplier,
                _stats.DecelerationDuration * clampedDistanceMultiplier,
                ExplosionDelay);
            _projectiles.Add(obj);
        }

        private static Sprite FallbackSprite
        {
            get
            {
                if (_fallbackSprite == null) _fallbackSprite = BandOrbSprite.Create(0.45f, false);
                return _fallbackSprite;
            }
        }
    }
}
