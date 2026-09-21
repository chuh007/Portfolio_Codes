using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    public sealed class TerrainRuleTileEditorWindow : EditorWindow
    {
        private readonly TerrainRuleTileEditorSession _session = new();
        private Vector2 _scroll;
        private TerrainType _newTerrainType = TerrainType.Ground;

        [MenuItem("Tools/Stage/Terrain Rule Tile Editor")]
        public static void Open()
        {
            TerrainRuleTileEditorWindow window = GetWindow<TerrainRuleTileEditorWindow>("Terrain Rule Tile");
            if (Selection.activeObject is TerrainRuleTile selectedTile)
                window._session.SetTile(selectedTile);

            window.Show();
        }

        private void OnDisable() => _session.ClearEditor();

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawTargetControls();
            EditorGUILayout.Space(8f);
            TerrainRuleTileEditorIcons.DrawSettings();
            EditorGUILayout.Space(8f);
            _session.Draw();
            EditorGUILayout.EndScrollView();
        }

        private void DrawTargetControls()
        {
            EditorGUILayout.LabelField("Terrain Rule Tile", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                TerrainRuleTile selectedTile = (TerrainRuleTile)EditorGUILayout.ObjectField(
                    "Tile",
                    _session.Tile,
                    typeof(TerrainRuleTile),
                    false);

                if (selectedTile != _session.Tile)
                    _session.SetTile(selectedTile);

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(!(Selection.activeObject is TerrainRuleTile)))
                    {
                        if (GUILayout.Button("Use Selection"))
                            _session.SetTile((TerrainRuleTile)Selection.activeObject);
                    }

                    _newTerrainType = (TerrainType)EditorGUILayout.EnumPopup(_newTerrainType, GUILayout.Width(110f));

                    if (GUILayout.Button("New Tile"))
                        CreateTerrainTileAsset(_newTerrainType);

                    if (GUILayout.Button("Create Set"))
                        CreateTerrainTileSet();
                }
            }
        }

        private void CreateTerrainTileAsset(TerrainType terrainType)
        {
            TerrainRuleTile tile = TerrainTileAssetCommands.Create(terrainType,
                TerrainTileAssetCommands.GetSelectedFolder(_session.Tile));
            if (tile == null) return;

            _session.SetTile(tile);
            Selection.activeObject = tile;
        }

        private void CreateTerrainTileSet()
        {
            TerrainRuleTile tile = TerrainTileAssetCommands.CreateSet(
                TerrainTileAssetCommands.GetSelectedFolder(_session.Tile));
            if (tile == null) return;

            _session.SetTile(tile);
            Selection.activeObject = tile;
        }
    }
}
