using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class SymphonicRockProjectiles
    {
        private readonly SymphonicRockAttack _source;
        private readonly SymphonicRockStats _stats;
        private readonly BandRuntimeObjects _projectiles;
        private static Sprite _boltSprite;

        public SymphonicRockProjectiles(SymphonicRockAttack source, SymphonicRockStats stats,
            BandRuntimeObjects projectiles)
        {
            _source = source;
            _stats = stats;
            _projectiles = projectiles;
        }

        public void SpawnBounceProjectile(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject projectileObject = ProjectilePool.Pop(
                "SymphonicRockBounceProjectile",
                _source.Position,
                Quaternion.Euler(0f, 0f, angle));
            if (projectileObject == null)
                return;

            projectileObject.name = "SymphonicRockBounceProjectile";
            projectileObject.transform.SetPositionAndRotation(
                _source.Position,
                Quaternion.Euler(0f, 0f, angle));
            projectileObject.transform.localScale = Vector3.one * 1.7f;

            SpriteRenderer renderer = projectileObject.GetComponentInChildren<SpriteRenderer>();
            if (renderer == null)
                renderer = projectileObject.AddComponent<SpriteRenderer>();
            if (renderer.sprite == null)
                renderer.sprite = BoltSprite;
            renderer.color = new Color(0.42f, 0.72f, 1f, 1f);
            ProjectileRenderLayer.ApplyTo(renderer, 49);

            var runtime = projectileObject.GetComponent<SymphonicRockBounceProjectile>()
                          ?? projectileObject.AddComponent<SymphonicRockBounceProjectile>();
            runtime.Init(
                _source,
                _source.OwnerTransform,
                direction,
                _stats.Speed,
                _source.ScaleCommonRange(_stats.MaxOwnerDistance),
                _source.ScaleCommonRange(_stats.BounceRange));
            _projectiles.Add(projectileObject);
        }

        private static Sprite BoltSprite
        {
            get
            {
                if (_boltSprite != null)
                    return _boltSprite;

                const int size = 32;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point
                };
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        bool core = Mathf.Abs(y - 16) <= 2 && x > 2 && x < 29;
                        bool spark = (x + y) % 11 <= 1 && Mathf.Abs(y - 16) < 7;
                        texture.SetPixel(x, y, core || spark ? Color.white : Color.clear);
                    }
                }
                texture.Apply();
                _boltSprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, size, size),
                    new Vector2(0.5f, 0.5f),
                    size);
                return _boltSprite;
            }
        }
    }
}
