using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    public sealed class TerrainRuleTileEditorSettings : ScriptableObject
    {
        private const string SettingsAssetPath = "Assets/_Work/CHUH/SO/CustomRuleTile/TerrainRuleTileEditorSettings.asset";

        private static TerrainRuleTileEditorSettings s_Instance;

        public Sprite groundIcon;
        public Sprite mintIcon;
        public Sprite waterIcon;
        public Sprite wallIcon;
        public Sprite anyTerrainIcon;
        public Sprite emptyIcon;

        public static TerrainRuleTileEditorSettings Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = AssetDatabase.LoadAssetAtPath<TerrainRuleTileEditorSettings>(SettingsAssetPath);
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = CreateInstance<TerrainRuleTileEditorSettings>();
                EnsureSettingsFolder();
                AssetDatabase.CreateAsset(s_Instance, SettingsAssetPath);
                AssetDatabase.SaveAssets();
                return s_Instance;
            }
        }

        public Sprite GetIcon(int neighbor)
        {
            return neighbor switch
            {
                TerrainRuleTile.Neighbor.Ground => groundIcon,
                TerrainRuleTile.Neighbor.Mint => mintIcon,
                TerrainRuleTile.Neighbor.Water => waterIcon,
                TerrainRuleTile.Neighbor.Wall => wallIcon,
                TerrainRuleTile.Neighbor.AnyTerrain => anyTerrainIcon,
                TerrainRuleTile.Neighbor.Empty => emptyIcon,
                _ => null
            };
        }

        public void SaveSettings()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureSettingsFolder()
        {
            string folder = Path.GetDirectoryName(SettingsAssetPath)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(folder) || AssetDatabase.IsValidFolder(folder))
                return;

            Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
        }
    }
}
