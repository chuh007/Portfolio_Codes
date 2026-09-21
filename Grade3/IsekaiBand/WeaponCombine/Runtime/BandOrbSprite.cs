using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal static class BandOrbSprite
    {
        public static Sprite Create(float edgePower, bool clampWrapMode)
        {
            const int size = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear
            };
            if (clampWrapMode) texture.wrapMode = TextureWrapMode.Clamp;
            Vector2 center = Vector2.one * ((size - 1) * 0.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                    float alpha = distance <= 1f ? Mathf.Pow(1f - distance, edgePower) : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}
