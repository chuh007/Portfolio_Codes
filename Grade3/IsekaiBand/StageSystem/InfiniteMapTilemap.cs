using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class InfiniteMapTilemap
    {
        private readonly Transform _owner;
        private readonly SpriteRenderer _background;
        private readonly int _groundLayer;
        private Transform _chunkParent;
        private Tilemap _tilemap;
        public Tilemap Tiles => _tilemap;

        public InfiniteMapTilemap(Transform owner, SpriteRenderer background)
        {
            _owner = owner;
            _background = background;
            _groundLayer = LayerMask.NameToLayer("Ground");
            if (_groundLayer < 0)
                _groundLayer = 0;
        }

        private void EnsureChunkParent()
        {
            if (_chunkParent != null) return;

            GameObject gridObject = new GameObject("InfiniteMapTileChunks");
            gridObject.transform.SetParent(_owner, true);
            float z = _background != null ? _background.transform.position.z : 0f;
            gridObject.transform.position = new Vector3(0f, 0f, z);
            gridObject.layer = _groundLayer;
            Grid grid = gridObject.AddComponent<Grid>();
            grid.cellSize = Vector3.one;
            _chunkParent = gridObject.transform;
        }

        public void Ensure()
        {
            if (_tilemap != null)
                return;

            EnsureChunkParent();
            GameObject tilemapObject = new GameObject("InfiniteMapTilemap");
            tilemapObject.transform.SetParent(_chunkParent, false);
            tilemapObject.layer = _groundLayer;
            _tilemap = tilemapObject.AddComponent<Tilemap>();
            tilemapObject.AddComponent<TilemapRenderer>();
            ApplySorting(_tilemap);
        }

        private void ApplySorting(Tilemap chunk)
        {
            if (!chunk.TryGetComponent(out TilemapRenderer renderer))
                renderer = chunk.gameObject.AddComponent<TilemapRenderer>();

            renderer.sortOrder = TilemapRenderer.SortOrder.BottomLeft;

            if (_background == null) return;

            renderer.sortingLayerID = _background.sortingLayerID;
            renderer.sortingOrder = _background.sortingOrder;
            renderer.sharedMaterial = _background.sharedMaterial;
        }

        public static void EnsureTilemapCollider(Tilemap tilemap)
        {
            if (tilemap == null)
                return;

            if (!tilemap.TryGetComponent(out TilemapCollider2D tilemapCollider))
                tilemapCollider = tilemap.gameObject.AddComponent<TilemapCollider2D>();

            tilemapCollider.enabled = true;
        }

        public static void SetTilemapColliderEnabled(Tilemap tilemap, bool enabled)
        {
            if (tilemap != null && tilemap.TryGetComponent(out TilemapCollider2D tilemapCollider))
                tilemapCollider.enabled = enabled;
        }
    }
}
