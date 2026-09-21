using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal readonly struct DecorationPlacementContext
    {
        private readonly InfiniteMapChunkLayout _layout;
        private readonly int _seed;
        private readonly StageDecorationProfileSO _profile;
        private readonly Vector2 _startPosition;
        private readonly Predicate<Vector3Int> _canPlace;

        public DecorationPlacementContext(
            InfiniteMapChunkLayout layout, int seed, StageDecorationProfileSO profile,
            Vector2 startPosition, Predicate<Vector3Int> canPlace)
        {
            _layout = layout;
            _seed = seed;
            _profile = profile;
            _startPosition = startPosition;
            _canPlace = canPlace;
        }

        public void GenerateGround(Vector2Int coord, List<GroundDecorationPlacement> results)
            => DecorationPlacementGenerator.GenerateGroundDecorations(
                coord, _layout.TileSize, _seed, _profile, _startPosition, _canPlace, results);

        public void GenerateLarge(Vector2Int coord, List<LargeDecorationPlacement> results)
            => DecorationPlacementGenerator.GenerateLargeDecorations(
                coord, _layout.TileSize, _seed, _profile, _startPosition, _canPlace, results);
    }
}
