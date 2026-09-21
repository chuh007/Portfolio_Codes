using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class InfiniteMapChunkGridPanel
    {
        private const int CellSize = 54;
        private readonly InfiniteMapChunkEditSession _session;
        private readonly TilePreviewGUI _preview;

        public InfiniteMapChunkGridPanel(InfiniteMapChunkEditSession session, TilePreviewGUI preview)
        {
            _session = session;
            _preview = preview;
        }

        public void Draw()
        {
            EditorGUILayout.LabelField($"Tile Grid {_session.Chunk.Width} x {_session.Chunk.Height}", EditorStyles.boldLabel);

            for (int y = _session.Chunk.Height - 1; y >= 0; y--)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int x = 0; x < _session.Chunk.Width; x++)
                    {
                        TileBase tile = _session.Chunk.GetTile(x, y);
                        Rect cellRect = GUILayoutUtility.GetRect(CellSize, CellSize, GUILayout.Width(CellSize), GUILayout.Height(CellSize));
                        Color previousColor = GUI.backgroundColor;
                        GUI.backgroundColor = tile != null ? Color.white : new Color(0.75f, 0.75f, 0.75f);

                        if (GUI.Button(cellRect, new GUIContent(string.Empty, tile != null ? tile.name : "Empty"), _preview.Style))
                            _session.PaintCell(x, y);

                        _preview.Draw(cellRect, tile);
                        GUI.backgroundColor = previousColor;
                    }
                }
            }
        }

        public void DrawValidation()
        {
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

            if (GUILayout.Button("Validate Chunk"))
                _session.Chunk.Validate(out _session.ValidationMessages);

            if (_session.ValidationMessages == null)
                return;

            if (_session.ValidationMessages.Count == 0)
            {
                EditorGUILayout.HelpBox("No validation errors.", MessageType.Info);
                return;
            }

            foreach (string message in _session.ValidationMessages)
            {
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }
        }
    }
}
