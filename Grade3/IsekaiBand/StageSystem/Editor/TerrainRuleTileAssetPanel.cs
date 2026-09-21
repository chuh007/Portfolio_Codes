using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class TerrainRuleTileAssetPanel
    {
        private static TerrainType s_NewTerrainType = TerrainType.Ground;
        private readonly UnityEditor.Editor _editor;

        public TerrainRuleTileAssetPanel(UnityEditor.Editor editor) => _editor = editor;

        public void Draw()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Asset Actions", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                s_NewTerrainType = (TerrainType)EditorGUILayout.EnumPopup("New Terrain Type", s_NewTerrainType);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Save"))
                        SaveSelectedTiles();

                    if (GUILayout.Button("New Tile"))
                        CreateTile();

                    using (new EditorGUI.DisabledScope(_editor.targets.Length != 1 || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(_editor.target))))
                    {
                        if (GUILayout.Button("Duplicate"))
                            TerrainTileAssetCommands.Duplicate(_editor.target);
                    }
                }

                if (GUILayout.Button("Create Ground / Mint / Water / Wall Tiles"))
                    TerrainTileAssetCommands.CreateSet(TerrainTileAssetCommands.GetSelectedFolder(_editor.target));
            }
        }

        private void SaveSelectedTiles()
        {
            foreach (Object selectedTarget in _editor.targets)
            {
                EditorUtility.SetDirty(selectedTarget);
            }

            AssetDatabase.SaveAssets();
        }

        private void CreateTile()
        {
            TerrainRuleTile tile = TerrainTileAssetCommands.Create(s_NewTerrainType,
                TerrainTileAssetCommands.GetSelectedFolder(_editor.target),
                _editor.targets.OfType<TerrainRuleTile>().FirstOrDefault());
            if (tile != null) Selection.activeObject = tile;
        }
    }
}
