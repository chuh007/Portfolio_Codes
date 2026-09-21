using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal static class MinimapIconFactory
    {
        public static void EnsureRoot(Transform transform, ref RectTransform iconRoot, MinimapIconSettings settings)
        {
            if (!settings.ShowSpriteIcons) return;

            RectTransform minimapRect = transform as RectTransform;
            if (minimapRect == null) return;

            if (transform.TryGetComponent(out RectMask2D mask))
                mask.enabled = settings.ClipToMinimap;
            else if (settings.ClipToMinimap)
                transform.gameObject.AddComponent<RectMask2D>();

            if (iconRoot != null) return;

            GameObject iconRootObject = new GameObject("MinimapIcons", typeof(RectTransform));
            iconRootObject.transform.SetParent(transform, false);
            iconRoot = iconRootObject.GetComponent<RectTransform>();
            iconRoot.anchorMin = Vector2.zero;
            iconRoot.anchorMax = Vector2.one;
            iconRoot.pivot = new Vector2(0.5f, 0.5f);
            iconRoot.offsetMin = Vector2.zero;
            iconRoot.offsetMax = Vector2.zero;
        }

        public static MinimapIconEntry Create(RectTransform iconRoot, Transform owner,
            SpriteRenderer renderer, Vector2 iconSize, MinimapPlayerIcon playerIcon)
        {
            GameObject iconObject = new GameObject($"{owner.name}_MinimapIcon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(iconRoot, false);

            RectTransform rect = iconObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = iconSize;

            Image image = iconObject.GetComponent<Image>();
            image.sprite = playerIcon.GetIconSprite(owner, renderer);
            image.raycastTarget = false;
            image.preserveAspect = true;

            return new MinimapIconEntry(owner, renderer, rect, image);
        }
    }
}
