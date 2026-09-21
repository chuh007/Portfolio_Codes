using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class GroundDecorationChunks
    {
        private readonly Dictionary<Vector2Int, List<Vector3Int>> _active = new();
        private readonly List<GroundDecorationPlacement> _placements = new();
        private readonly InfiniteMapDecorationSurface _surface;

        public GroundDecorationChunks(InfiniteMapDecorationSurface surface) => _surface = surface;

        public bool Add(Vector2Int coord, DecorationPlacementContext context)
        {
            context.GenerateGround(coord, _placements);
            var occupied = new List<Vector3Int>(_placements.Count);
            foreach (GroundDecorationPlacement placement in _placements)
            {
                _surface.Tiles.SetTile(placement.Cell, placement.Tile);
                _surface.Tiles.SetTransformMatrix(
                    placement.Cell,
                    placement.FlipX ? Matrix4x4.Scale(new Vector3(-1f, 1f, 1f)) : Matrix4x4.identity);
                occupied.Add(placement.Cell);
            }
            _active.Add(coord, occupied);
            return occupied.Count > 0;
        }

        public bool Remove(Vector2Int coord)
        {
            if (!_active.Remove(coord, out List<Vector3Int> occupied)) return false;

            foreach (Vector3Int cell in occupied)
                _surface.Tiles.SetTile(cell, null);
            return occupied.Count > 0;
        }

        public void Clear()
        {
            if (_surface.Tiles != null)
                _surface.Tiles.ClearAllTiles();
            _active.Clear();
            _placements.Clear();
        }
    }
}
