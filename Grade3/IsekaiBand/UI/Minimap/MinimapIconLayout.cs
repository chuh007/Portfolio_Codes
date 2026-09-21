using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal sealed class MinimapIconLayout
    {
        private readonly MinimapWeaponIndicator _weaponIndicator;

        public MinimapIconLayout(MinimapWeaponIndicator weaponIndicator) => _weaponIndicator = weaponIndicator;

        public void Update(Camera minimapCamera, RectTransform iconRoot, MinimapIconSettings settings,
            Transform transform, IEnumerable<MinimapIconEntry> entries)
        {
            if (minimapCamera == null || iconRoot == null) return;

            RectTransform minimapRect = transform as RectTransform;
            if (minimapRect == null) return;

            Vector2 rectSize = minimapRect.rect.size;
            foreach (MinimapIconEntry entry in entries)
            {
                if (!IsVisibleEntry(entry))
                {
                    entry.Image.enabled = false;
                    MinimapWeaponIndicator.SetEnabled(entry, false);
                    continue;
                }

                Vector3 viewportPoint = minimapCamera.WorldToViewportPoint(entry.Owner.position);
                bool inView = viewportPoint.z > 0f
                              && viewportPoint.x >= 0f
                              && viewportPoint.x <= 1f
                              && viewportPoint.y >= 0f
                              && viewportPoint.y <= 1f;

                if (!inView && IsWeapon(entry, settings.WeaponLayerMask) && settings.ShowWeaponIndicators)
                {
                    _weaponIndicator.Update(entry, viewportPoint, rectSize, minimapCamera, iconRoot, settings);
                    continue;
                }

                if (!inView && settings.HideOutsideView)
                {
                    entry.Image.enabled = false;
                    MinimapWeaponIndicator.SetEnabled(entry, false);
                    continue;
                }

                viewportPoint.x = Mathf.Clamp01(viewportPoint.x);
                viewportPoint.y = Mathf.Clamp01(viewportPoint.y);

                entry.Image.enabled = true;
                MinimapWeaponIndicator.SetEnabled(entry, false);
                entry.Rect.anchoredPosition = new Vector2(
                    (viewportPoint.x - 0.5f) * rectSize.x,
                    (viewportPoint.y - 0.5f) * rectSize.y);
            }
        }

        private static bool IsVisibleEntry(MinimapIconEntry entry)
        {
            return entry.Owner != null
                   && entry.Owner.gameObject.activeInHierarchy
                   && MinimapIconCandidates.IsUsableRenderer(entry.Renderer);
        }

        public static bool IsWeapon(MinimapIconEntry entry, LayerMask offscreenWeaponLayerMask)
        {
            return entry.Owner != null
                   && (offscreenWeaponLayerMask.value & (1 << entry.Owner.gameObject.layer)) != 0;
        }
    }
}
