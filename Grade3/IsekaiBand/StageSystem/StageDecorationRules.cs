using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    [Serializable]
    public sealed class GroundDecorationRule
    {
        [SerializeField] private TileBase tile;
        [SerializeField, Min(1)] private int weight = 1;
        [SerializeField] private bool allowFlipX = true;

        public TileBase Tile => tile;
        public int Weight => Mathf.Max(1, weight);
        public bool AllowFlipX => allowFlipX;
        public bool IsUsable => tile != null;
    }

    [Serializable]
    public sealed class LargeDecorationRule
    {
        [SerializeField] private GameObject prefab;
        [SerializeField, Min(1)] private int weight = 1;
        [SerializeField] private bool allowFlipX = true;
        [SerializeField, Min(0)] private int terrainClearanceRadius = 1;

        public GameObject Prefab => prefab;
        public int Weight => Mathf.Max(1, weight);
        public bool AllowFlipX => allowFlipX;
        public int TerrainClearanceRadius => Mathf.Max(0, terrainClearanceRadius);
        public bool IsUsable => prefab != null;
    }
}
