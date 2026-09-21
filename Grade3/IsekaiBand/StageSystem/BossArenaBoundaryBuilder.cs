using System.Collections.Generic;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaBoundaryBuilder
    {
        private readonly Transform _owner;
        private readonly SpriteRenderer _background;
        private readonly int _wallLayer;
        private readonly Vector2 _arenaSize;
        private readonly float _wallThickness;
        private readonly Color _wallColor;
        private readonly BossArenaVisualResources _resources;
        private GameObject _parent;

        public GameObject Parent => _parent;
        public Vector2 Size => new Vector2(Mathf.Max(1f, _arenaSize.x), Mathf.Max(1f, _arenaSize.y));

        public BossArenaBoundaryBuilder(
            Transform owner, SpriteRenderer background, int wallLayer, Vector2 arenaSize,
            Sprite cornerSprite, Material chainMaterial, float wallThickness, Color wallColor)
        {
            _owner = owner;
            _background = background;
            _wallLayer = wallLayer;
            _arenaSize = arenaSize;
            _wallThickness = wallThickness;
            _wallColor = wallColor;
            _resources = new BossArenaVisualResources(cornerSprite, chainMaterial);
        }

        public void Prepare()
        {
            if (_parent != null)
                return;

            Vector2 arenaSize = Size;
            _parent = new GameObject("BossArenaBoundaries");
            _parent.transform.SetParent(_owner, false);
            _parent.SetActive(false);

            float thickness = Mathf.Max(0.1f, _wallThickness);
            float halfWidth = arenaSize.x * 0.5f;
            float halfHeight = arenaSize.y * 0.5f;
            float outerHalfWidth = halfWidth + thickness * 0.5f;
            float outerHalfHeight = halfHeight + thickness * 0.5f;

            CreateWallCollider(
                "TopWall",
                new Vector2(0f, halfHeight + thickness * 0.5f),
                new Vector2(arenaSize.x + thickness * 2f, thickness));

            CreateWallCollider(
                "BottomWall",
                new Vector2(0f, -halfHeight - thickness * 0.5f),
                new Vector2(arenaSize.x + thickness * 2f, thickness));

            CreateWallCollider(
                "LeftWall",
                new Vector2(-halfWidth - thickness * 0.5f, 0f),
                new Vector2(thickness, arenaSize.y));

            CreateWallCollider(
                "RightWall",
                new Vector2(halfWidth + thickness * 0.5f, 0f),
                new Vector2(thickness, arenaSize.y));

            var visuals = new BossArenaBoundaryVisuals(_parent, _background, _wallLayer, _wallColor, _resources);
            Vector2 topLeft = new Vector2(-outerHalfWidth, outerHalfHeight);
            Vector2 topRight = new Vector2(outerHalfWidth, outerHalfHeight);
            Vector2 bottomLeft = new Vector2(-outerHalfWidth, -outerHalfHeight);
            Vector2 bottomRight = new Vector2(outerHalfWidth, -outerHalfHeight);

            visuals.CreateSide("Top", topLeft, topRight, thickness, 7, true, true);
            visuals.CreateSide("Right", topRight, bottomRight, thickness, 5, false, true);
            visuals.CreateSide("Bottom", bottomRight, bottomLeft, thickness, 7, false, true);
            visuals.CreateSide("Left", bottomLeft, topLeft, thickness, 5, false, false);
        }

        private void CreateWallCollider(string wallName, Vector2 localPosition, Vector2 size)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(_parent.transform, false);
            wall.transform.localPosition = localPosition;
            wall.layer = _wallLayer;

            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
        }

        public void Dispose()
        {
            if (_parent != null)
                Object.Destroy(_parent);
            _resources.Dispose();
            _parent = null;
        }
    }
}
