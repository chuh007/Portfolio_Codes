using UnityEngine;

namespace _Work.CHUH.Code.UI.Minimap
{
    internal sealed class MinimapArrowSprite
    {
        private Sprite _arrowSprite;

        public Sprite Get()
        {
            if (_arrowSprite != null)
                return _arrowSprite;

            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;

            Color clear = new Color(1f, 1f, 1f, 0f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                    texture.SetPixel(x, y, clear);
            }

            Vector2 top = new Vector2(size * 0.5f, size - 4f);
            Vector2 left = new Vector2(8f, 8f);
            Vector2 right = new Vector2(size - 8f, 8f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 point = new Vector2(x + 0.5f, y + 0.5f);
                    if (IsPointInTriangle(point, top, left, right))
                        texture.SetPixel(x, y, Color.white);
                }
            }

            texture.Apply();
            _arrowSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            return _arrowSprite;
        }

        private static bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = Sign(point, a, b);
            float d2 = Sign(point, b, c);
            float d3 = Sign(point, c, a);

            bool hasNegative = d1 < 0f || d2 < 0f || d3 < 0f;
            bool hasPositive = d1 > 0f || d2 > 0f || d3 > 0f;
            return !(hasNegative && hasPositive);
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }
    }
}
