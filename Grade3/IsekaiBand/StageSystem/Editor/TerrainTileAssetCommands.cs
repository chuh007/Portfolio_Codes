using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal static class TerrainTileAssetCommands
    {
        public static TerrainRuleTile Create(TerrainType terrainType, string folder, TerrainRuleTile sourceTile = null)
        {
            string defaultName = $"{terrainType}TerrainTile.asset";
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Terrain Rule Tile",
                defaultName,
                "asset",
                "Choose where to save the terrain rule tile.",
                folder);

            if (string.IsNullOrEmpty(path))
                return null;

            TerrainRuleTile newTile = ScriptableObject.CreateInstance<TerrainRuleTile>();
            newTile.terrainType = terrainType;

            if (sourceTile != null)
            {
                newTile.m_DefaultColliderType = sourceTile.m_DefaultColliderType;
                newTile.m_DefaultSprite = sourceTile.m_DefaultSprite;
            }

            AssetDatabase.CreateAsset(newTile, path);
            AssetDatabase.SaveAssets();
            return newTile;
        }

        public static void Duplicate(Object target)
        {
            string sourcePath = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(sourcePath))
                return;

            string folder = Path.GetDirectoryName(sourcePath)?.Replace('\\', '/') ?? "Assets";
            string fileName = Path.GetFileNameWithoutExtension(sourcePath);
            string newPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{fileName} Copy.asset");

            if (!AssetDatabase.CopyAsset(sourcePath, newPath))
                return;

            AssetDatabase.SaveAssets();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<TerrainRuleTile>(newPath);
        }

        public static TerrainRuleTile CreateSet(string folder)
        {
            TerrainRuleTile lastCreatedTile = null;

            foreach (TerrainType terrainType in System.Enum.GetValues(typeof(TerrainType)))
            {
                string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{terrainType}TerrainTile.asset");
                TerrainRuleTile newTile = ScriptableObject.CreateInstance<TerrainRuleTile>();
                newTile.terrainType = terrainType;
                AssetDatabase.CreateAsset(newTile, path);
                lastCreatedTile = newTile;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return lastCreatedTile;
        }

        public static string GetSelectedFolder(Object primary)
        {
            if (primary != null)
            {
                string tilePath = AssetDatabase.GetAssetPath(primary);
                string tileFolder = GetFolderFromPath(tilePath);
                if (!string.IsNullOrEmpty(tileFolder))
                    return tileFolder;
            }

            foreach (Object selectedObject in Selection.objects)
            {
                string folder = GetFolderFromPath(AssetDatabase.GetAssetPath(selectedObject));
                if (!string.IsNullOrEmpty(folder))
                    return folder;
            }

            return "Assets";
        }

        private static string GetFolderFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (AssetDatabase.IsValidFolder(path))
                return path;

            return Path.GetDirectoryName(path)?.Replace('\\', '/');
        }
    }
}
