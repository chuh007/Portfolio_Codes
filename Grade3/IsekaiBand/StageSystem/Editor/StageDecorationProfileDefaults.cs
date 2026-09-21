using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal static class StageDecorationProfileDefaults
    {
        private const string PrefabFolder =
            "Assets/_Graphics/2D Hand Painted Tilesets/2D Hand Painted - Grassland Tileset/Prefabs";
        private const string TreePrefabPath = PrefabFolder + "/Tree.prefab";
        private const string SignPrefabPath = PrefabFolder + "/Sign.prefab";
        private static readonly int[] DecorationWeights = { 2, 2, 3, 2, 3, 2, 3, 3 };

        public static void ConfigureGround(StageDecorationProfileSO profile, IReadOnlyList<Tile> tiles)
        {
            SerializedObject serializedProfile = new SerializedObject(profile);
            serializedProfile.FindProperty("seedOffset").intValue = 104729;
            serializedProfile.FindProperty("spacing").intValue = 2;
            serializedProfile.FindProperty("spawnChancePerBlock").floatValue = 0.45f;
            serializedProfile.FindProperty("startSafeRadius").floatValue = 4f;

            SerializedProperty rules = serializedProfile.FindProperty("groundDecorations");
            rules.arraySize = tiles.Count;
            for (int i = 0; i < tiles.Count; i++)
            {
                SerializedProperty rule = rules.GetArrayElementAtIndex(i);
                rule.FindPropertyRelative("tile").objectReferenceValue = tiles[i];
                rule.FindPropertyRelative("weight").intValue = DecorationWeights[i];
                rule.FindPropertyRelative("allowFlipX").boolValue = true;
            }

            serializedProfile.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(profile);
        }

        public static void ConfigureLargeDecorationsIfMissing(StageDecorationProfileSO profile)
        {
            SerializedObject serializedProfile = new SerializedObject(profile);
            SerializedProperty rules = serializedProfile.FindProperty("largeDecorations");
            if (rules == null || rules.arraySize > 0)
                return;

            GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TreePrefabPath);
            GameObject signPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SignPrefabPath);
            if (treePrefab == null || signPrefab == null)
            {
                Debug.LogWarning("[StageDecoration] Tree or Sign prefab is missing.");
                return;
            }

            serializedProfile.FindProperty("largeSpacing").intValue = 8;
            serializedProfile.FindProperty("largeSpawnChancePerBlock").floatValue = 0.2f;
            serializedProfile.FindProperty("largeStartSafeRadius").floatValue = 7f;
            serializedProfile.FindProperty("largeBlockPadding").intValue = 2;

            rules.arraySize = 2;
            ConfigureLargeRule(rules.GetArrayElementAtIndex(0), treePrefab, 1, 2);
            ConfigureLargeRule(rules.GetArrayElementAtIndex(1), signPrefab, 2, 1);

            serializedProfile.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(profile);
        }

        private static void ConfigureLargeRule(
            SerializedProperty rule,
            GameObject prefab,
            int weight,
            int terrainClearanceRadius)
        {
            rule.FindPropertyRelative("prefab").objectReferenceValue = prefab;
            rule.FindPropertyRelative("weight").intValue = weight;
            rule.FindPropertyRelative("allowFlipX").boolValue = true;
            rule.FindPropertyRelative("terrainClearanceRadius").intValue = terrainClearanceRadius;
        }
    }
}
