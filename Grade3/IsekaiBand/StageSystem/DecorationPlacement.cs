using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    public readonly struct GroundDecorationPlacement
    {
        public GroundDecorationPlacement(Vector3Int cell, TileBase tile, bool flipX)
        {
            Cell = cell;
            Tile = tile;
            FlipX = flipX;
        }

        public Vector3Int Cell { get; }
        public TileBase Tile { get; }
        public bool FlipX { get; }
    }

    public readonly struct LargeDecorationPlacement
    {
        public LargeDecorationPlacement(Vector3Int cell, GameObject prefab, bool flipX)
        {
            Cell = cell;
            Prefab = prefab;
            FlipX = flipX;
        }

        public Vector3Int Cell { get; }
        public GameObject Prefab { get; }
        public bool FlipX { get; }
    }
}
