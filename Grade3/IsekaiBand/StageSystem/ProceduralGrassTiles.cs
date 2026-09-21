using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class ProceduralGrassTiles
    {
        private const string TileResourcePrefix = "GrasslandColor3Tiles/GrasslandColor3_";
        private const int BaseTileIdA = 24;
        private const int BaseTileIdB = 69;
        private const int BaseTileIdC = 70;
        private readonly Dictionary<int, TileBase> _grassTiles = new();
        private bool _tilesLoaded;

        public bool Load()
        {
            if (_tilesLoaded)
                return true;

            _grassTiles.Clear();
            int[] tileIds =
            {
                24, 69, 70
            };

            foreach (int tileId in tileIds)
            {
                TileBase tile = Resources.Load<TileBase>($"{TileResourcePrefix}{tileId}");
                if (tile == null)
                {
                    Debug.LogError($"[InfiniteMapChunkRenderer] Missing Grassland Color 3 tile: {TileResourcePrefix}{tileId}");
                    return false;
                }

                _grassTiles.Add(tileId, tile);
            }

            _tilesLoaded = true;
            return true;
        }

        public TileBase Pick(int x, int y)
        {
            uint hash = InfiniteMapChunkLayout.Hash(x, y, 1597334677);
            uint roll = hash % 100u;

            if (roll < 72u)
                return _grassTiles[BaseTileIdA];

            if (roll < 86u)
                return _grassTiles[BaseTileIdB];

            return _grassTiles[BaseTileIdC];
        }
    }
}
