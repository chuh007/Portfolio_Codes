using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    [CustomEditor(typeof(TerrainRuleTile), true)]
    [CanEditMultipleObjects]
    public sealed class TerrainRuleTileEditor : RuleTileEditor
    {
        private TerrainRuleTileAssetPanel _assets;
        private TerrainRuleTileSpritePanel _sprites;

        public override void OnInspectorGUI()
        {
            DrawTerrainRuleHelp();
            (_assets ??= new TerrainRuleTileAssetPanel(this)).Draw();
            (_sprites ??= new TerrainRuleTileSpritePanel(this)).Draw();
            EditorGUILayout.Space(8f);

            base.OnInspectorGUI();
        }

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            if (TerrainRuleTileEditorIcons.DrawTerrainRuleIcon(rect, neighbor))
                return;

            base.RuleOnGUI(rect, position, neighbor);
        }

        private void DrawTerrainRuleHelp()
        {
            EditorGUILayout.LabelField("Terrain Rule Conditions", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawConditionRow("0", "Don't Care", "Ignore this neighbor.");
                DrawConditionRow("1", "This", "Same tile asset as this RuleTile.");
                DrawConditionRow("2", "Not This", "Anything except this exact tile asset, including empty.");
                DrawConditionRow("3", "Ground", "TerrainRuleTile with terrainType = Ground.");
                DrawConditionRow("4", "Mint", "TerrainRuleTile with terrainType = Mint.");
                DrawConditionRow("5", "Water", "TerrainRuleTile with terrainType = Water.");
                DrawConditionRow("6", "Wall", "TerrainRuleTile with terrainType = Wall.");
                DrawConditionRow("7", "AnyTerrain", "Any TerrainRuleTile.");
                DrawConditionRow("8", "Empty", "No tile in that cell.");
            }

            EditorGUILayout.HelpBox(
                "Use Ground/Mint/Water/Wall for terrain transitions. Configure rule icons in Tools/Stage/Terrain Rule Tile Editor.",
                MessageType.Info);

            if (GUILayout.Button("Open Terrain Rule Tile Editor"))
                TerrainRuleTileEditorWindow.Open();
        }

        private static void DrawConditionRow(string id, string label, string description)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(id, EditorStyles.miniBoldLabel, GUILayout.Width(24f));
                GUILayout.Label(label, EditorStyles.miniBoldLabel, GUILayout.Width(82f));
                GUILayout.Label(description, EditorStyles.miniLabel);
            }
        }
    }
}
