using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal static class StageDecorationDefaultsInstaller
    {
        private const string SessionKey = "BandRoguelike.StageDecorationDefaultsInstalled";

        [InitializeOnLoadMethod]
        private static void ScheduleDefaultAssetInstall()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += EnsureDefaultGrasslandDecorations;
        }

        [MenuItem("Tools/Stage/Ensure Default Grassland Decorations")]
        public static void EnsureDefaultGrasslandDecorations()
        {
            List<Tile> tiles = StageDecorationDefaultAssets.LoadTiles();
            StageDecorationProfileSO profile = AssetDatabase.LoadAssetAtPath<StageDecorationProfileSO>(StageDecorationDefaultAssets.ProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<StageDecorationProfileSO>();
                profile.name = "DefaultGrasslandDecorationProfile";
                AssetDatabase.CreateAsset(profile, StageDecorationDefaultAssets.ProfilePath);
                StageDecorationProfileDefaults.ConfigureGround(profile, tiles);
            }
            else if (!profile.HasGroundDecorations)
            {
                StageDecorationProfileDefaults.ConfigureGround(profile, tiles);
            }

            StageDecorationProfileDefaults.ConfigureLargeDecorationsIfMissing(profile);
            AssignProfileToGrasslandStages(profile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void AssignProfileToGrasslandStages(StageDecorationProfileSO profile)
        {
            foreach (string guid in AssetDatabase.FindAssets("t:StageDataSO"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                StageDataSO stage = AssetDatabase.LoadAssetAtPath<StageDataSO>(path);
                if (stage == null || stage.DecorationProfile != null || stage.BackGroundSprite == null)
                    continue;

                if (AssetDatabase.GetAssetPath(stage.BackGroundSprite) != StageDecorationDefaultAssets.SheetPath)
                    continue;

                stage.DecorationProfile = profile;
                EditorUtility.SetDirty(stage);
            }
        }
    }
}
