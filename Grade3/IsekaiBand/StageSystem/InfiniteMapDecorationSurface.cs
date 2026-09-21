using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class InfiniteMapDecorationSurface
    {
        private readonly Transform _owner;
        private readonly SpriteRenderer _background;
        private readonly int _groundLayer;
        private Transform _root;
        private Tilemap _tilemap;
        public Transform Root => _root;
        public Tilemap Tiles => _tilemap;

        public InfiniteMapDecorationSurface(Transform owner, SpriteRenderer background)
        {
            _owner = owner;
            _background = background;
            int layer = LayerMask.NameToLayer("Ground");
            _groundLayer = layer >= 0 ? layer : 0;
        }

        public void Ensure()
        {
            if (_tilemap != null)
                return;

            GameObject gridObject = new GameObject("InfiniteMapDecorations");
            gridObject.transform.SetParent(_owner, true);
            float z = _background != null ? _background.transform.position.z : 0f;
            gridObject.transform.position = new Vector3(0f, 0f, z);
            gridObject.layer = _groundLayer;
            gridObject.AddComponent<Grid>().cellSize = Vector3.one;
            _root = gridObject.transform;

            GameObject tilemapObject = new GameObject("GroundDecorationTilemap");
            tilemapObject.transform.SetParent(_root, false);
            tilemapObject.layer = _groundLayer;
            _tilemap = tilemapObject.AddComponent<Tilemap>();
            TilemapRenderer renderer = tilemapObject.AddComponent<TilemapRenderer>();
            renderer.sortOrder = TilemapRenderer.SortOrder.BottomLeft;

            if (_background == null)
                return;

            renderer.sortingLayerID = _background.sortingLayerID;
            renderer.sortingOrder = _background.sortingOrder + 1;
            renderer.sharedMaterial = _background.sharedMaterial;
        }
    }
}
