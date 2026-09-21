using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class TilePreviewGUI
    {
        private readonly Action _repaint;
        private GUIStyle _cellStyle;
        public GUIStyle Style => _cellStyle;

        public TilePreviewGUI(Action repaint) => _repaint = repaint;

        public void EnsureStyles()
        {
            _cellStyle ??= new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                wordWrap = true
            };
        }

        public void Draw(Rect cellRect, TileBase tile, float alpha = 1f)
        {
            Rect contentRect = new Rect(
                cellRect.x + 5f,
                cellRect.y + 5f,
                cellRect.width - 10f,
                cellRect.height - 10f);

            if (tile == null)
            {
                GUI.Label(contentRect, "-", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            if (StageTilePreview.TryGetTileSprite(tile, out Sprite sprite))
            {
                StageTilePreview.DrawSpritePreview(contentRect, sprite, alpha);
                return;
            }

            Texture preview = AssetPreview.GetAssetPreview(tile);
            if (preview == null)
                preview = AssetPreview.GetMiniThumbnail(tile);

            if (preview != null)
            {
                Color previousColor = GUI.color;
                GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, previousColor.a * alpha);
                GUI.DrawTexture(contentRect, preview, ScaleMode.ScaleToFit, true);
                GUI.color = previousColor;
                return;
            }

            if (AssetPreview.IsLoadingAssetPreview(tile.GetInstanceID()))
                _repaint();

            GUI.Label(contentRect, ShortName(tile.name), EditorStyles.centeredGreyMiniLabel);
        }

        private static string ShortName(string value)
        {
            const int maxLength = 9;
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength - 1) + ".";
        }
    }
}
