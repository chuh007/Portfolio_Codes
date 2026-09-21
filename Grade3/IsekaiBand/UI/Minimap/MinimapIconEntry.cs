using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal sealed class MinimapIconEntry
    {
        public readonly Transform Owner;
        public SpriteRenderer Renderer;
        public readonly RectTransform Rect;
        public readonly Image Image;
        public RectTransform ArrowRect;
        public Image ArrowImage;

        public MinimapIconEntry(Transform owner, SpriteRenderer renderer, RectTransform rect, Image image)
        {
            Owner = owner;
            Renderer = renderer;
            Rect = rect;
            Image = image;
        }
    }
}
