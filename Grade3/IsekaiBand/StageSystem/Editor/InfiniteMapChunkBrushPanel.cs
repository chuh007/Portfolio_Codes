using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class InfiniteMapChunkBrushPanel
    {
        private readonly InfiniteMapChunkEditSession _session;
        private readonly TilePreviewGUI _preview;

        public InfiniteMapChunkBrushPanel(InfiniteMapChunkEditSession session, TilePreviewGUI preview)
        {
            _session = session;
            _preview = preview;
        }

        public void Draw()
        {
            EditorGUILayout.LabelField("Brush", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                TileBase selectedBrush = (TileBase)EditorGUILayout.ObjectField(
                    "TileBase Brush",
                    _session.Brush,
                    typeof(TileBase),
                    false);
                if (selectedBrush != _session.Brush)
                    _session.Brush = selectedBrush;

                TerrainRuleTile selectedTerrainBrush = (TerrainRuleTile)EditorGUILayout.ObjectField(
                    "Terrain RuleTile Brush",
                    _session.Brush as TerrainRuleTile,
                    typeof(TerrainRuleTile),
                    false);
                if (selectedTerrainBrush != _session.Brush as TerrainRuleTile)
                    _session.Brush = selectedTerrainBrush;

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(!(Selection.activeObject is TileBase)))
                    {
                        if (GUILayout.Button("Use Selected Tile"))
                            _session.Brush = (TileBase)Selection.activeObject;
                    }

                    using (new EditorGUI.DisabledScope(_session.Brush == null))
                    {
                        if (GUILayout.Button("Clear Brush", GUILayout.Width(100f)))
                            _session.Brush = null;
                    }
                }

                DrawBrushPreview();
            }

            EditorGUILayout.HelpBox("Click the tile grid to paint with the selected tile.", MessageType.None);
        }

        private void DrawBrushPreview()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Rect previewRect = GUILayoutUtility.GetRect(62f, 62f, GUILayout.Width(62f), GUILayout.Height(62f));
                GUI.Box(previewRect, GUIContent.none, _preview.Style);
                _preview.Draw(previewRect, _session.Brush);

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Selected Brush", EditorStyles.miniBoldLabel);
                    EditorGUILayout.LabelField(_session.Brush != null ? _session.Brush.name : "None", EditorStyles.miniLabel);

                    if (_session.Brush is TerrainRuleTile terrainRuleTile)
                        EditorGUILayout.LabelField($"Terrain: {terrainRuleTile.terrainType}", EditorStyles.miniLabel);
                }
            }
        }
    }
}
