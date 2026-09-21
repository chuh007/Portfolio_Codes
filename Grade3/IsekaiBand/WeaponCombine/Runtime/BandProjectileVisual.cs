using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class BandProjectileVisual
    {
        private static Sprite _orbSprite;

        public static GameObject SpawnProjectile(
            string resourcePath,
            string objectName,
            Vector3 position,
            Quaternion rotation,
            Color color,
            float scale,
            int sortingOrder = 51)
        {
            GameObject obj = ProjectilePool.Pop(objectName, position, rotation);
            if (obj == null)
                return null;

            obj.name = objectName;
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.transform.localScale = Vector3.one * scale;

            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
            if (renderers.Length == 0)
                renderers = new[] { obj.AddComponent<SpriteRenderer>() };

            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer.sprite == null)
                    renderer.sprite = OrbSprite;
                renderer.color = color;
                ProjectileRenderLayer.ApplyTo(renderer, sortingOrder);
            }

            ProjectileRenderLayer.ApplyTo(obj);
            return obj;
        }

        private static Sprite OrbSprite
        {
            get
            {
                if (_orbSprite == null) _orbSprite = BandOrbSprite.Create(0.42f, true);
                return _orbSprite;
            }
        }
    }
}
