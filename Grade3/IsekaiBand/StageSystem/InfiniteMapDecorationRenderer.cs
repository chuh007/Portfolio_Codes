using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    public sealed class InfiniteMapDecorationRenderer
    {
        private const int FallbackStageSeed = 1597334677;
        private readonly Func<Transform> _playerProvider;
        private readonly Predicate<Vector3Int> _canPlace;
        private readonly int _visibleChunkRadius;
        private readonly InfiniteMapVisibleChunks _visible = new();
        private readonly InfiniteMapDecorationSurface _surface;
        private readonly GroundDecorationChunks _ground;
        private readonly LargeDecorationChunks _large;
        private readonly Func<Vector2Int, bool> _addChunk;
        private readonly Action<Vector2Int> _removeChunk;
        private StageDecorationProfileSO _profile;
        private InfiniteMapChunkLayout _layout;
        private DecorationPlacementContext _placement;
        private bool _tilemapChanged;
        private bool _collidersChanged;

        public InfiniteMapDecorationRenderer(
            Transform owner, SpriteRenderer background, Func<Transform> playerProvider,
            Predicate<Vector3Int> canPlace, int visibleChunkRadius)
        {
            _playerProvider = playerProvider;
            _canPlace = canPlace;
            _visibleChunkRadius = Mathf.Max(0, visibleChunkRadius);
            _surface = new InfiniteMapDecorationSurface(owner, background);
            _ground = new GroundDecorationChunks(_surface);
            _large = new LargeDecorationChunks(new LargeDecorationPool(_surface));
            _addChunk = AddChunk;
            _removeChunk = RemoveChunk;
        }

        public void Setup(StageDataSO stageData)
        {
            _ground.Clear();
            _large.Clear();
            _visible.Clear();
            _profile = null;
            Physics2D.SyncTransforms();
            _profile = stageData != null ? stageData.DecorationProfile : null;
            if (stageData == null || _profile == null
                                  || (!_profile.HasGroundDecorations && !_profile.HasLargeDecorations))
                return;

            _layout = new InfiniteMapChunkLayout(stageData);
            int seed = _layout.UsesLibrary ? stageData.ChunkLibrary.Seed : FallbackStageSeed;
            Transform player = _playerProvider?.Invoke();
            Vector2 start = player != null ? (Vector2)player.position : Vector2.zero;
            _placement = new DecorationPlacementContext(_layout, seed, _profile, start, _canPlace);
            _surface.Ensure();
            Refresh();
        }

        public void Refresh()
        {
            if (_profile == null || (!_profile.HasGroundDecorations && !_profile.HasLargeDecorations)) return;

            _surface.Ensure();
            Transform player = _playerProvider?.Invoke();
            Vector2 center = player != null ? (Vector2)player.position : Vector2.zero;
            _tilemapChanged = false;
            _collidersChanged = false;
            _visible.Refresh(_layout.WorldToChunk(center), _visibleChunkRadius, _addChunk, _removeChunk);
            if (_tilemapChanged)
            {
                _surface.Tiles.RefreshAllTiles();
                _surface.Tiles.CompressBounds();
            }
            if (_collidersChanged)
                Physics2D.SyncTransforms();
        }

        public void ClearLargeDecorations(Rect worldArea) => _large.ClearArea(worldArea);

        private bool AddChunk(Vector2Int coord)
        {
            _tilemapChanged |= _ground.Add(coord, _placement);
            _collidersChanged |= _large.Add(coord, _placement);
            return true;
        }

        private void RemoveChunk(Vector2Int coord)
        {
            _tilemapChanged |= _ground.Remove(coord);
            _collidersChanged |= _large.Remove(coord);
        }
    }
}
