using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal static class TerrainRuleTileEditorIcons
    {
        public static bool DrawTerrainRuleIcon(Rect rect, int neighbor)
        {
            if (!TryGetRuleInfo(neighbor, out string tooltip, out Color fallbackColor))
                return false;

            Rect iconRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, rect.height - 4f);
            Sprite icon = TerrainRuleTileEditorSettings.Instance.GetIcon(neighbor);

            EditorGUI.DrawRect(iconRect, fallbackColor);
            if (icon != null)
                StageTilePreview.DrawSpriteIcon(iconRect, icon);

            DrawBorder(iconRect, new Color(0f, 0f, 0f, 0.35f));
            GUI.Label(rect, new GUIContent(string.Empty, tooltip));
            return true;
        }

        private static bool TryGetRuleInfo(int neighbor, out string tooltip, out Color fallbackColor)
        {
            switch (neighbor)
            {
                case TerrainRuleTile.Neighbor.Ground:
                    tooltip = "Ground";
                    fallbackColor = new Color(0.28f, 0.55f, 0.24f);
                    return true;
                case TerrainRuleTile.Neighbor.Mint:
                    tooltip = "Mint";
                    fallbackColor = new Color(0.22f, 0.72f, 0.58f);
                    return true;
                case TerrainRuleTile.Neighbor.Water:
                    tooltip = "Water";
                    fallbackColor = new Color(0.20f, 0.42f, 0.78f);
                    return true;
                case TerrainRuleTile.Neighbor.Wall:
                    tooltip = "Wall";
                    fallbackColor = new Color(0.38f, 0.38f, 0.38f);
                    return true;
                case TerrainRuleTile.Neighbor.AnyTerrain:
                    tooltip = "AnyTerrain";
                    fallbackColor = new Color(0.62f, 0.46f, 0.20f);
                    return true;
                case TerrainRuleTile.Neighbor.Empty:
                    tooltip = "Empty";
                    fallbackColor = new Color(0.18f, 0.18f, 0.18f);
                    return true;
                default:
                    tooltip = string.Empty;
                    fallbackColor = Color.clear;
                    return false;
            }
        }

        private static void DrawBorder(Rect rect, Color color)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1f, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1f, rect.y, 1f, rect.height), color);
        }

        public static void DrawSettings()
        {
            EditorGUILayout.LabelField("Rule Condition Icons", EditorStyles.boldLabel);

            TerrainRuleTileEditorSettings settings = TerrainRuleTileEditorSettings.Instance;
            EditorGUI.BeginChangeCheck();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                settings.groundIcon = DrawIconField("Ground (3)", settings.groundIcon);
                settings.mintIcon = DrawIconField("Mint (4)", settings.mintIcon);
                settings.waterIcon = DrawIconField("Water (5)", settings.waterIcon);
                settings.wallIcon = DrawIconField("Wall (6)", settings.wallIcon);
                settings.anyTerrainIcon = DrawIconField("AnyTerrain (7)", settings.anyTerrainIcon);
                settings.emptyIcon = DrawIconField("Empty (8)", settings.emptyIcon);
            }

            if (EditorGUI.EndChangeCheck())
            {
                settings.SaveSettings();
                foreach (UnityEditor.Editor editor in Resources.FindObjectsOfTypeAll<UnityEditor.Editor>())
                    editor.Repaint();
            }
        }

        private static Sprite DrawIconField(string label, Sprite sprite)
        {
            return (Sprite)EditorGUILayout.ObjectField(label, sprite, typeof(Sprite), false);
        }
    }
}
