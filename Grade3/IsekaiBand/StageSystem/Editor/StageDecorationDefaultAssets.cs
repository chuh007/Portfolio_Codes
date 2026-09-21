using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal static class StageDecorationDefaultAssets
    {
        public const string SheetPath =
            "Assets/_Graphics/2D Hand Painted Tilesets/2D Hand Painted - Grassland Tileset/" +
            "Sprites/Tilesets 64x64/Grassland Color 3@64x64.png";
        public const string RootFolder = "Assets/_Work/CHUH/SO/Stage/Decoration";
        public const string TileFolder = RootFolder + "/Tiles";
        public const string ProfilePath = RootFolder + "/DefaultGrasslandDecorationProfile.asset";
        private static readonly int[] DecorationSpriteIndices = { 33, 34, 35, 130, 131, 228, 240, 244 };

        public static List<Tile> LoadTiles()
        {
            EnsureFolder(RootFolder);
            EnsureFolder(TileFolder);
            Dictionary<string, Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(SheetPath)
                .OfType<Sprite>()
                .ToDictionary(sprite => sprite.name);

            List<Tile> tiles = new List<Tile>(DecorationSpriteIndices.Length);
            foreach (int spriteIndex in DecorationSpriteIndices)
            {
                string spriteName = $"GrasslandColor3@64x64_{spriteIndex}";
                if (!sprites.TryGetValue(spriteName, out Sprite sprite))
                {
                    Debug.LogWarning($"[StageDecoration] Missing sprite: {spriteName}");
                    continue;
                }

                string tilePath = $"{TileFolder}/GrasslandColor3_Decor_{spriteIndex}.asset";
                Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
                if (tile == null)
                {
                    tile = ScriptableObject.CreateInstance<Tile>();
                    tile.name = $"GrasslandColor3_Decor_{spriteIndex}";
                    tile.sprite = sprite;
                    tile.colliderType = Tile.ColliderType.None;
                    tile.flags = TileFlags.LockColor;
                    AssetDatabase.CreateAsset(tile, tilePath);
                }

                tiles.Add(tile);
            }
            return tiles;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
                return;

            int split = folderPath.LastIndexOf('/');
            string parent = folderPath[..split];
            string name = folderPath[(split + 1)..];
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
