using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    public class InfiniteMapChunkRenderer
    {
        private readonly SpriteRenderer _background;
        private readonly Func<Transform> _playerProvider;
        private readonly int _visibleChunkRadius;
        private readonly InfiniteMapVisibleChunks _visible = new();
        private readonly InfiniteMapChunkSelection _selection = new();
        private readonly ProceduralGrassTiles _grass = new();
        private readonly InfiniteMapTilemap _surface;
        private readonly InfiniteMapChunkPainter _painter;
        private readonly Func<Vector2Int, bool> _addChunk;
        private readonly Action<Vector2Int> _removeChunk;
        private StageDataSO _stageData;
        private InfiniteMapChunkLayout _layout;

        public InfiniteMapChunkRenderer(
            Transform owner, SpriteRenderer background, Func<Transform> playerProvider, int visibleChunkRadius)
        {
            _background = background;
            _playerProvider = playerProvider;
            _visibleChunkRadius = Mathf.Max(0, visibleChunkRadius);
            _surface = new InfiniteMapTilemap(owner, background);
            _painter = new InfiniteMapChunkPainter(_surface, _grass);
            _addChunk = AddChunk;
            _removeChunk = _painter.ClearChunk;
        }

        public void Setup(StageDataSO stageData)
        {
            if (stageData == null) return;

            if (_surface.Tiles != null)
                _surface.Tiles.ClearAllTiles();
            _visible.Clear();
            _selection.Setup(stageData);
            _stageData = stageData;
            _layout = new InfiniteMapChunkLayout(stageData);
            _painter.Setup(_layout);

            if (_background != null)
            {
                if (stageData.BackGroundSprite != null)
                    _background.sprite = stageData.BackGroundSprite;
                _background.gameObject.SetActive(false);
            }

            _surface.Ensure();
            InfiniteMapTilemap.SetTilemapColliderEnabled(_surface.Tiles, _layout.UsesLibrary);
            Refresh();
        }

        public void Refresh()
        {
            if (_stageData == null) return;
            if (!_layout.UsesLibrary && !_grass.Load()) return;

            _surface.Ensure();
            Transform player = _playerProvider?.Invoke();
            Vector2 center = player != null ? (Vector2)player.position : Vector2.zero;
            if (!_visible.Refresh(_layout.WorldToChunk(center), _visibleChunkRadius, _addChunk, _removeChunk))
                return;

            _surface.Tiles.RefreshAllTiles();
            _surface.Tiles.CompressBounds();
        }

        public bool CanPlaceGroundDecoration(Vector3Int cell) => _painter.CanPlaceGroundDecoration(cell);

        private bool AddChunk(Vector2Int coord)
        {
            if (_layout.UsesLibrary)
            {
                InfiniteMapChunkDataSO chunkData = _selection.Select(coord);
                if (chunkData == null) return false;
                _painter.FillAuthoredChunk(coord, chunkData);
            }
            else
            {
                _painter.FillProceduralChunk(coord);
            }
            return true;
        }
    }
}
