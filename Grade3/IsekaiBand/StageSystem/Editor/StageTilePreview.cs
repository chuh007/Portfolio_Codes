using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal static class StageTilePreview
    {
        public static bool TryGetTileSprite(TileBase tile, out Sprite sprite)
        {
            switch (tile)
            {
                case TerrainRuleTile terrainRuleTile:
                    sprite = terrainRuleTile.m_DefaultSprite;
                    return sprite != null;
                case RuleTile ruleTile:
                    sprite = ruleTile.m_DefaultSprite;
                    return sprite != null;
                case Tile regularTile:
                    sprite = regularTile.sprite;
                    return sprite != null;
                default:
                    sprite = null;
                    return false;
            }
        }

        public static void DrawSpritePreview(Rect rect, Sprite sprite, float alpha)
        {
            Texture2D texture = sprite.texture;
            if (texture == null)
                return;

            Rect textureRect = sprite.textureRect;
            Rect sourceRect = new Rect(
                textureRect.x / texture.width,
                textureRect.y / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height);
            Rect fittedRect = GetAspectFitRect(rect, textureRect.width / textureRect.height);

            Color previousColor = GUI.color;
            GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * alpha);
            GUI.DrawTextureWithTexCoords(fittedRect, texture, sourceRect, true);
            GUI.color = previousColor;
        }

        public static void DrawSpriteIcon(Rect rect, Sprite sprite)
        {
            Texture2D texture = sprite.texture;
            if (texture == null)
                return;

            try
            {
                Rect textureRect = sprite.textureRect;
                Rect sourceRect = new Rect(
                    textureRect.x / texture.width,
                    textureRect.y / texture.height,
                    textureRect.width / texture.width,
                    textureRect.height / texture.height);
                Rect fittedRect = GetAspectFitRect(rect, textureRect.width / textureRect.height);
                GUI.DrawTextureWithTexCoords(fittedRect, texture, sourceRect, true);
            }
            catch (System.Exception)
            {
                Texture preview = AssetPreview.GetAssetPreview(sprite);
                if (preview == null)
                    preview = AssetPreview.GetMiniThumbnail(sprite);

                if (preview != null)
                    GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit, true);
            }
        }

        private static Rect GetAspectFitRect(Rect rect, float aspect)
        {
            if (aspect <= 0f)
                return rect;

            float rectAspect = rect.width / rect.height;
            if (rectAspect > aspect)
            {
                float width = rect.height * aspect;
                return new Rect(rect.x + (rect.width - width) * 0.5f, rect.y, width, rect.height);
            }

            float height = rect.width / aspect;
            return new Rect(rect.x, rect.y + (rect.height - height) * 0.5f, rect.width, height);
        }
    }
}
