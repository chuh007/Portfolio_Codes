using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal static class TerrainRuleSpriteCommands
    {
        public static void ApplyDefaultSprite(Object[] targets, Sprite sprite)
        {
            foreach (TerrainRuleTile selectedTile in targets.OfType<TerrainRuleTile>())
            {
                Undo.RecordObject(selectedTile, "Set Terrain Rule Tile Default Sprite");
                selectedTile.m_DefaultSprite = sprite;
                EditorUtility.SetDirty(selectedTile);
            }
        }

        public static void AddSpriteRules(Object[] targets, IReadOnlyList<Sprite> sprites)
        {
            if (sprites == null || sprites.Count == 0)
                return;

            foreach (TerrainRuleTile selectedTile in targets.OfType<TerrainRuleTile>())
            {
                Undo.RecordObject(selectedTile, "Add Terrain Rule Tile Sprite Rules");
                foreach (Sprite sprite in sprites)
                {
                    if (sprite == null)
                        continue;

                    RuleTile.TilingRule rule = new RuleTile.TilingRule
                    {
                        m_Id = GetNextRuleId(selectedTile),
                        m_Sprites = new[] { sprite },
                        m_ColliderType = selectedTile.m_DefaultColliderType
                    };

                    selectedTile.m_TilingRules.Add(rule);
                }

                selectedTile.UpdateNeighborPositions();
                EditorUtility.SetDirty(selectedTile);
            }
        }

        private static int GetNextRuleId(RuleTile tile)
        {
            int nextId = 0;
            foreach (RuleTile.TilingRule rule in tile.m_TilingRules)
            {
                if (rule != null && rule.m_Id >= nextId)
                    nextId = rule.m_Id + 1;
            }

            return nextId;
        }

        public static Sprite[] GetSpritesFromObjects(Object[] objects)
        {
            if (objects == null || objects.Length == 0)
                return System.Array.Empty<Sprite>();

            HashSet<Sprite> sprites = new HashSet<Sprite>();
            foreach (Object obj in objects)
            {
                if (obj == null)
                    continue;

                if (obj is Sprite sprite)
                {
                    sprites.Add(sprite);
                    continue;
                }

                string path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                    continue;

                foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    if (asset is Sprite loadedSprite)
                        sprites.Add(loadedSprite);
                }
            }

            return sprites.ToArray();
        }
    }
}
