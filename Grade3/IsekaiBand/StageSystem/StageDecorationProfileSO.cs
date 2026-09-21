using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    [CreateAssetMenu(
        fileName = "StageDecorationProfile",
        menuName = "SO/Stage/Decoration Profile",
        order = 4)]
    public sealed class StageDecorationProfileSO : ScriptableObject
    {
        [Header("Deterministic Placement")]
        [SerializeField] private int seedOffset = 104729;
        [SerializeField, Min(1)] private int spacing = 2;
        [SerializeField, Range(0f, 1f)] private float spawnChancePerBlock = 0.45f;
        [SerializeField, Min(0f)] private float startSafeRadius = 4f;

        [Header("Ground Decorations")]
        [SerializeField] private List<GroundDecorationRule> groundDecorations = new();

        [Header("Large Collidable Decorations")]
        [SerializeField, Min(1)] private int largeSpacing = 8;
        [SerializeField, Range(0f, 1f)] private float largeSpawnChancePerBlock = 0.2f;
        [SerializeField, Min(0f)] private float largeStartSafeRadius = 7f;
        [SerializeField, Min(0)] private int largeBlockPadding = 2;
        [SerializeField] private List<LargeDecorationRule> largeDecorations = new();

        public int SeedOffset => seedOffset;
        public int Spacing => Mathf.Max(1, spacing);
        public float SpawnChancePerBlock => Mathf.Clamp01(spawnChancePerBlock);
        public float StartSafeRadius => Mathf.Max(0f, startSafeRadius);
        public int LargeSpacing => Mathf.Max(1, largeSpacing);
        public float LargeSpawnChancePerBlock => Mathf.Clamp01(largeSpawnChancePerBlock);
        public float LargeStartSafeRadius => Mathf.Max(0f, largeStartSafeRadius);
        public int LargeBlockPadding => Mathf.Clamp(largeBlockPadding, 0, Mathf.Max(0, (LargeSpacing - 1) / 2));

        public bool HasGroundDecorations => DecorationRuleSelector.HasGround(groundDecorations);

        public bool HasLargeDecorations => DecorationRuleSelector.HasLarge(largeDecorations);

        public GroundDecorationRule SelectGroundDecoration(uint roll)
            => DecorationRuleSelector.SelectGround(groundDecorations, roll);

        public LargeDecorationRule SelectLargeDecoration(uint roll)
            => DecorationRuleSelector.SelectLarge(largeDecorations, roll);

        private void OnValidate()
        {
            spacing = Mathf.Max(1, spacing);
            spawnChancePerBlock = Mathf.Clamp01(spawnChancePerBlock);
            startSafeRadius = Mathf.Max(0f, startSafeRadius);
            largeSpacing = Mathf.Max(1, largeSpacing);
            largeSpawnChancePerBlock = Mathf.Clamp01(largeSpawnChancePerBlock);
            largeStartSafeRadius = Mathf.Max(0f, largeStartSafeRadius);
            largeBlockPadding = Mathf.Clamp(largeBlockPadding, 0, Mathf.Max(0, (largeSpacing - 1) / 2));

            if (groundDecorations == null)
                groundDecorations = new List<GroundDecorationRule>();

            if (largeDecorations == null)
                largeDecorations = new List<LargeDecorationRule>();
        }
    }
}
