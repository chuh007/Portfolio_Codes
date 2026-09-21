using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    public sealed class InfiniteMapChunkEditorWindow : EditorWindow
    {
        private readonly InfiniteMapChunkEditSession _session = new();
        private InfiniteMapChunkAssetPanel _assets;
        private InfiniteMapChunkBrushPanel _brush;
        private InfiniteMapChunkGridPanel _grid;
        private TilePreviewGUI _preview;
        private Vector2 _scroll;

        [MenuItem("Tools/Stage/Infinite Map Chunk Editor")]
        private static void Open()
        {
            GetWindow<InfiniteMapChunkEditorWindow>("Map Chunk Editor");
        }

        private void OnGUI()
        {
            EnsurePanels();
            _preview.EnsureStyles();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _assets.Draw();
            EditorGUILayout.Space(8f);

            if (_session.Chunk == null)
            {
                EditorGUILayout.HelpBox("Select or create an InfiniteMapChunk asset.", MessageType.Info);
                EditorGUILayout.EndScrollView();
                return;
            }

            DrawChunkControls();
            EditorGUILayout.Space(8f);
            _brush.Draw();
            EditorGUILayout.Space(8f);
            _grid.Draw();
            EditorGUILayout.Space(8f);
            _grid.DrawValidation();
            EditorGUILayout.EndScrollView();
        }

        private void EnsurePanels()
        {
            if (_preview != null) return;

            _preview = new TilePreviewGUI(Repaint);
            _assets = new InfiniteMapChunkAssetPanel(_session);
            _brush = new InfiniteMapChunkBrushPanel(_session, _preview);
            _grid = new InfiniteMapChunkGridPanel(_session, _preview);
        }

        private void DrawChunkControls()
        {
            EditorGUILayout.LabelField("Chunk", EditorStyles.boldLabel);

            int width = EditorGUILayout.IntField("Width", _session.Chunk.Width);
            int height = EditorGUILayout.IntField("Height", _session.Chunk.Height);
            int spawnWeight = EditorGUILayout.IntField("Spawn Weight", _session.Chunk.SpawnWeight);
            width = Mathf.Max(1, width);
            height = Mathf.Max(1, height);
            spawnWeight = Mathf.Max(0, spawnWeight);

            if (width != _session.Chunk.Width || height != _session.Chunk.Height)
            {
                Undo.RecordObject(_session.Chunk, "Resize Infinite Map Chunk");
                _session.Chunk.Resize(width, height);
                EditorUtility.SetDirty(_session.Chunk);
                _session.ValidationMessages = null;
            }

            if (spawnWeight != _session.Chunk.SpawnWeight)
            {
                Undo.RecordObject(_session.Chunk, "Change Infinite Map Chunk Spawn Weight");
                _session.Chunk.SetSpawnWeight(spawnWeight);
                EditorUtility.SetDirty(_session.Chunk);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Resize 8 x 8"))
                    _session.ResizeChunk(InfiniteMapChunkDataSO.DefaultSize, InfiniteMapChunkDataSO.DefaultSize);

                if (GUILayout.Button("Fill With Brush"))
                    _session.FillWithBrush();

                if (GUILayout.Button("Clear Tiles"))
                    _session.ClearChunk();
            }
        }
    }
}
