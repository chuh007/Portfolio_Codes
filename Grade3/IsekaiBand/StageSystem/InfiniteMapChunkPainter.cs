using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class InfiniteMapChunkPainter
    {
        private readonly InfiniteMapTilemap _surface;
        private readonly ProceduralGrassTiles _grass;
        private InfiniteMapChunkLayout _layout;
        private Tilemap _tilemap => _surface.Tiles;

        public InfiniteMapChunkPainter(InfiniteMapTilemap surface, ProceduralGrassTiles grass)
        {
            _surface = surface;
            _grass = grass;
        }

        public void Setup(InfiniteMapChunkLayout layout) => _layout = layout;

        public bool CanPlaceGroundDecoration(Vector3Int cell)
        {
            if (_tilemap == null)
                return false;

            TileBase tile = _tilemap.GetTile(cell);
            if (tile == null || _tilemap.GetColliderType(cell) != Tile.ColliderType.None)
                return false;

            if (tile is not TerrainRuleTile terrainTile)
                return true;

            return terrainTile.terrainType is TerrainType.Ground or TerrainType.Mint;
        }

        public void FillProceduralChunk(Vector2Int coord)
        {
            InfiniteMapTilemap.SetTilemapColliderEnabled(_tilemap, false);
            Vector3Int start = _layout.StartCell(coord);

            for (int x = 0; x < _layout.TileSize.x; x++)
            {
                for (int y = 0; y < _layout.TileSize.y; y++)
                {
                    Vector3Int cell = new Vector3Int(start.x + x, start.y + y, 0);
                    _tilemap.SetTile(cell, _grass.Pick(cell.x, cell.y));
                }
            }
        }

        public void FillAuthoredChunk(Vector2Int coord, InfiniteMapChunkDataSO chunkData)
        {
            InfiniteMapTilemap.EnsureTilemapCollider(_tilemap);
            Vector3Int start = _layout.StartCell(coord);

            for (int x = 0; x < chunkData.Width; x++)
            {
                for (int y = 0; y < chunkData.Height; y++)
                {
                    TileBase tile = chunkData.GetTile(x, y);
                    if (tile == null)
                        continue;

                    Vector3Int cell = new Vector3Int(start.x + x, start.y + y, 0);
                    _tilemap.SetTile(cell, tile);
                }
            }
        }

        public void ClearChunk(Vector2Int coord)
        {
            Vector3Int start = _layout.StartCell(coord);
            for (int x = 0; x < _layout.TileSize.x; x++)
            {
                for (int y = 0; y < _layout.TileSize.y; y++)
                {
                    _tilemap.SetTile(new Vector3Int(start.x + x, start.y + y, 0), null);
                }
            }
        }
    }
}
