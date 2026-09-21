using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal sealed class MinimapSpriteIcons
    {
        private readonly Transform _transform;
        private readonly Dictionary<Transform, MinimapIconEntry> _icons = new();
        private readonly List<Transform> _iconsToRemove = new();
        private readonly MinimapIconCandidates _candidates = new();
        private readonly MinimapPlayerIcon _playerIcon = new();
        private readonly MinimapWeaponIndicator _weaponIndicator = new();
        private readonly MinimapIconLayout _layout;
        private float _nextIconRefreshTime;

        public MinimapSpriteIcons(Transform transform)
        {
            _transform = transform;
            _layout = new MinimapIconLayout(_weaponIndicator);
        }

        public void Update(Camera camera, ref RectTransform iconRoot, MinimapIconSettings settings)
        {
            if (!settings.ShowSpriteIcons)
            {
                ClearSpriteIcons();
                return;
            }

            MinimapIconFactory.EnsureRoot(_transform, ref iconRoot, settings);
            if (Time.unscaledTime >= _nextIconRefreshTime)
                Refresh(iconRoot, settings);

            _layout.Update(camera, iconRoot, settings, _transform, _icons.Values);
        }

        public void Refresh(RectTransform iconRoot, MinimapIconSettings settings)
        {
            _nextIconRefreshTime = Time.unscaledTime + settings.RefreshInterval;
            IReadOnlyDictionary<Transform, SpriteRenderer> candidates = _candidates.Gather(settings.LayerMask);
            _iconsToRemove.Clear();
            foreach (var icon in _icons)
            {
                if (!candidates.ContainsKey(icon.Key))
                    _iconsToRemove.Add(icon.Key);
            }

            foreach (Transform owner in _iconsToRemove)
            {
                if (_icons.TryGetValue(owner, out MinimapIconEntry entry))
                    DestroyIcon(entry);

                _icons.Remove(owner);
            }

            foreach (var candidate in candidates)
                EnsureIcon(candidate.Key, candidate.Value, iconRoot, settings);
        }

        private void EnsureIcon(Transform owner, SpriteRenderer renderer,
            RectTransform iconRoot, MinimapIconSettings settings)
        {
            if (iconRoot == null || owner == null || renderer == null) return;

            if (!_icons.TryGetValue(owner, out MinimapIconEntry entry))
            {
                entry = MinimapIconFactory.Create(iconRoot, owner, renderer, settings.IconSize, _playerIcon);
                _icons.Add(owner, entry);
            }

            entry.Renderer = renderer;
            entry.Rect.sizeDelta = settings.IconSize;
            entry.Image.color = Color.white;

            if (_playerIcon.IsPlayerIconOwner(owner))
                entry.Image.sprite = _playerIcon.GetIconSprite(owner, renderer);

            if (MinimapIconLayout.IsWeapon(entry, settings.WeaponLayerMask))
                _weaponIndicator.Ensure(entry, iconRoot, settings);
        }

        private void ClearSpriteIcons()
        {
            foreach (var icon in _icons)
                DestroyIcon(icon.Value);

            _icons.Clear();
        }

        private static void DestroyIcon(MinimapIconEntry entry)
        {
            if (entry.Image != null)
                Object.Destroy(entry.Image.gameObject);

            if (entry.ArrowImage != null)
                Object.Destroy(entry.ArrowImage.gameObject);
        }
    }
}
