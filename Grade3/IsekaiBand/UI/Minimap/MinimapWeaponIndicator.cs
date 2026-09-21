using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal sealed class MinimapWeaponIndicator
    {
        private readonly MinimapArrowSprite _sprite = new();

        public void Update(MinimapIconEntry entry, Vector3 viewportPoint, Vector2 rectSize,
            Camera minimapCamera, RectTransform iconRoot, MinimapIconSettings settings)
        {
            Vector2 direction = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f);
            if (direction.sqrMagnitude <= 0.0001f)
                direction = ((Vector2)entry.Owner.position - (Vector2)minimapCamera.transform.position).normalized;

            if (direction.sqrMagnitude <= 0.0001f)
                direction = Vector2.up;

            Vector2 edgePosition = GetEdgePosition(direction, rectSize);
            Vector2 edgeDirection = edgePosition.sqrMagnitude > 0.0001f
                ? edgePosition.normalized
                : direction.normalized;

            entry.Image.enabled = true;
            entry.Rect.anchoredPosition = edgePosition + edgeDirection * settings.WeaponIconOffset;

            Ensure(entry, iconRoot, settings);
            SetEnabled(entry, true);

            entry.ArrowRect.sizeDelta = settings.WeaponArrowSize;
            entry.ArrowRect.anchoredPosition = edgePosition + edgeDirection * settings.WeaponArrowOffset;

            float angle = Mathf.Atan2(edgeDirection.y, edgeDirection.x) * Mathf.Rad2Deg;
            entry.ArrowRect.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            entry.ArrowImage.color = settings.WeaponArrowColor;
        }

        private static Vector2 GetEdgePosition(Vector2 direction, Vector2 rectSize)
        {
            Vector2 halfSize = rectSize * 0.5f;
            float xScale = Mathf.Abs(direction.x) > 0.0001f
                ? halfSize.x / Mathf.Abs(direction.x)
                : float.PositiveInfinity;
            float yScale = Mathf.Abs(direction.y) > 0.0001f
                ? halfSize.y / Mathf.Abs(direction.y)
                : float.PositiveInfinity;

            return direction * Mathf.Min(xScale, yScale);
        }

        public void Ensure(MinimapIconEntry entry, RectTransform iconRoot, MinimapIconSettings settings)
        {
            if (entry.ArrowImage != null) return;
            if (iconRoot == null) return;

            GameObject arrowObject = new GameObject($"{entry.Owner.name}_WeaponDirectionArrow", typeof(RectTransform), typeof(Image));
            arrowObject.transform.SetParent(iconRoot, false);

            RectTransform rect = arrowObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = settings.WeaponArrowSize;

            Image image = arrowObject.GetComponent<Image>();
            image.sprite = _sprite.Get();
            image.raycastTarget = false;
            image.preserveAspect = true;
            image.color = settings.WeaponArrowColor;
            image.enabled = false;

            entry.ArrowRect = rect;
            entry.ArrowImage = image;
        }

        public static void SetEnabled(MinimapIconEntry entry, bool enabled)
        {
            if (entry.ArrowImage != null)
                entry.ArrowImage.enabled = enabled;
        }
    }
}
