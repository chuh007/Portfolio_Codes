using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class TerrainRuleTileEditorSession
    {
        public TerrainRuleTile Tile { get; private set; }
        private UnityEditor.Editor _tileEditor;

        public void Draw()
        {
            if (Tile == null)
            {
                EditorGUILayout.HelpBox("Select a TerrainRuleTile or create a new one.", MessageType.Info);
                return;
            }

            UnityEditor.Editor.CreateCachedEditor(Tile, null, ref _tileEditor);
            _tileEditor.OnInspectorGUI();
        }

        public void SetTile(TerrainRuleTile tile)
        {
            if (Tile == tile)
                return;

            Tile = tile;
            ClearEditor();
        }

        public void ClearEditor()
        {
            if (_tileEditor == null)
                return;

            Object.DestroyImmediate(_tileEditor);
            _tileEditor = null;
        }
    }
}
