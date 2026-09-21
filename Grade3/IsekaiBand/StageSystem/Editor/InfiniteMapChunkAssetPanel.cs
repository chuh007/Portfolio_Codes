using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class InfiniteMapChunkAssetPanel
    {
        private readonly InfiniteMapChunkEditSession _session;

        public InfiniteMapChunkAssetPanel(InfiniteMapChunkEditSession session) => _session = session;

        public void Draw()
        {
            EditorGUILayout.LabelField("Assets", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                _session.Chunk = (InfiniteMapChunkDataSO)EditorGUILayout.ObjectField(
                    "Chunk",
                    _session.Chunk,
                    typeof(InfiniteMapChunkDataSO),
                    false);

                if (GUILayout.Button("New", GUILayout.Width(72f)))
                    CreateChunkAsset();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _session.Library = (InfiniteMapChunkLibrarySO)EditorGUILayout.ObjectField(
                    "Library",
                    _session.Library,
                    typeof(InfiniteMapChunkLibrarySO),
                    false);

                if (GUILayout.Button("New", GUILayout.Width(72f)))
                    CreateLibraryAsset();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Chunk To Library"))
                    AddChunkToLibrary();

                if (GUILayout.Button("Save Assets", GUILayout.Width(100f)))
                    AssetDatabase.SaveAssets();
            }

            if (_session.Chunk != null && _session.Library != null && !_session.Chunk.HasSameSize(_session.Library.ChunkTileSize))
            {
                EditorGUILayout.HelpBox(
                    $"Chunk size {_session.Chunk.Width}x{_session.Chunk.Height} does not match library size {_session.Library.ChunkTileSize.x}x{_session.Library.ChunkTileSize.y}.",
                    MessageType.Warning);
            }
        }

        private void CreateChunkAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Infinite Map Chunk",
                "InfiniteMapChunk.asset",
                "asset",
                "Choose where to save the chunk asset.");

            if (string.IsNullOrEmpty(path))
                return;

            InfiniteMapChunkDataSO chunk = ScriptableObject.CreateInstance<InfiniteMapChunkDataSO>();
            chunk.Resize(InfiniteMapChunkDataSO.DefaultSize, InfiniteMapChunkDataSO.DefaultSize);
            AssetDatabase.CreateAsset(chunk, path);
            AssetDatabase.SaveAssets();
            _session.Chunk = chunk;
            Selection.activeObject = chunk;
        }

        private void CreateLibraryAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Infinite Map Chunk Library",
                "InfiniteMapChunkLibrary.asset",
                "asset",
                "Choose where to save the library asset.");

            if (string.IsNullOrEmpty(path))
                return;

            InfiniteMapChunkLibrarySO library = ScriptableObject.CreateInstance<InfiniteMapChunkLibrarySO>();
            AssetDatabase.CreateAsset(library, path);
            AssetDatabase.SaveAssets();
            _session.Library = library;
            Selection.activeObject = library;
        }

        private void AddChunkToLibrary()
        {
            if (_session.Chunk == null || _session.Library == null)
                return;

            Undo.RecordObject(_session.Library, "Add Infinite Map Chunk To Library");
            _session.Library.AddChunk(_session.Chunk);
            EditorUtility.SetDirty(_session.Library);
        }
    }
}
