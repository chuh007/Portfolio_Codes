using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    /// <summary>
    /// 공연장 조합별 바닥 스프라이트의 로드, 크기 보정, 플레이어 추적과 수명을 관리한다.
    /// </summary>
    internal sealed class VenueGroundVisual : MonoBehaviour
    {
        private const float VenueAlpha = 0.18f;
        private const int SortingOrder = -20;

        private static readonly Dictionary<CombineWeaponType, Sprite> SpriteCache = new();

        private Transform _owner;
        private float _spriteSide;

        public static VenueGroundVisual Create(
            CombineWeaponType combinationType,
            Transform owner,
            float radius)
        {
            if (owner == null || radius <= 0f)
                return null;

            string spritePath = GetSpritePath(combinationType);
            if (string.IsNullOrWhiteSpace(spritePath))
                return null;

            Sprite sprite = LoadSprite(combinationType, spritePath);
            if (sprite == null)
                return null;

            var visualObject = new GameObject($"{combinationType}VenueGround");
            var renderer = visualObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(1f, 1f, 1f, VenueAlpha);
            GroundEffectRenderLayer.ApplyTo(renderer, SortingOrder);

            var visual = visualObject.AddComponent<VenueGroundVisual>();
            visual.Initialize(owner, radius, sprite);
            return visual;
        }

        public void Release()
        {
            if (gameObject != null)
                Destroy(gameObject);
        }

        private void Initialize(Transform owner, float radius, Sprite sprite)
        {
            _owner = owner;
            FollowOwner();

            Vector2 spriteSize = sprite.bounds.size;
            _spriteSide = Mathf.Max(spriteSize.x, spriteSize.y);
            SetRadius(radius);
        }

        internal void SetRadius(float radius)
        {
            float scale = _spriteSide > 0f ? radius * 2f / _spriteSide : 1f;
            transform.localScale = Vector3.one * scale;
        }

        private void LateUpdate()
        {
            if (_owner == null)
            {
                Destroy(gameObject);
                return;
            }

            FollowOwner();
        }

        private void FollowOwner()
        {
            Vector3 ownerPosition = _owner.position;
            transform.position = new Vector3(ownerPosition.x, ownerPosition.y, ownerPosition.z);
        }

        private static Sprite LoadSprite(CombineWeaponType combinationType, string spritePath)
        {
            if (SpriteCache.TryGetValue(combinationType, out Sprite cachedSprite))
                return cachedSprite;

            Sprite sprite = Resources.Load<Sprite>(spritePath);
            SpriteCache[combinationType] = sprite;
            if (sprite == null)
                Debug.LogWarning($"Venue ground sprite could not be loaded: {spritePath}");
            return sprite;
        }

        private static string GetSpritePath(CombineWeaponType combinationType)
            => combinationType switch
            {
                CombineWeaponType.OrthodoxRockBand =>
                    "LCH/RuntimeSprites/Venues/OrthodoxRockVenue",
                CombineWeaponType.JazzPopBand =>
                    "LCH/RuntimeSprites/Venues/JazzPopVenue",
                CombineWeaponType.FusionJazzBand =>
                    "LCH/RuntimeSprites/Venues/FusionJazzVenue",
                CombineWeaponType.EmotionalRockBand =>
                    "LCH/RuntimeSprites/Venues/EmotionalRockVenue",
                _ => null
            };
    }
}
