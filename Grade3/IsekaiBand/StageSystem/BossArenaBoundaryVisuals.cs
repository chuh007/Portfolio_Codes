using System.Collections.Generic;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaBoundaryVisuals
    {
        private readonly GameObject _parent;
        private readonly SpriteRenderer _background;
        private readonly int _wallLayer;
        private readonly Color _wallColor;
        private readonly BossArenaVisualResources _resources;

        public BossArenaBoundaryVisuals(
            GameObject parent, SpriteRenderer background, int wallLayer, Color wallColor, BossArenaVisualResources resources)
        {
            _parent = parent;
            _background = background;
            _wallLayer = wallLayer;
            _wallColor = wallColor;
            _resources = resources;
        }

        public void CreateSide(
            string sideName,
            Vector2 from,
            Vector2 to,
            float width,
            int wandCount,
            bool createStartWand,
            bool createEndWand)
        {
            Vector2[] points = new Vector2[wandCount];

            for (int i = 0; i < wandCount; i++)
            {
                points[i] = Vector2.Lerp(from, to, i / (float)(wandCount - 1));

                bool isStart = i == 0;
                bool isEnd = i == wandCount - 1;
                if ((isStart && !createStartWand) || (isEnd && !createEndWand))
                    continue;

                CreateWand($"{sideName}Wand_{i}", points[i], width);
            }

            CreateChain($"{sideName}Chain", points, width);
        }

        private void CreateChain(string chainName, Vector2[] points, float width)
        {
            GameObject chain = new GameObject(chainName);
            chain.transform.SetParent(_parent.transform, false);
            chain.layer = _wallLayer;

            LineRenderer renderer = chain.AddComponent<LineRenderer>();
            renderer.useWorldSpace = false;
            renderer.positionCount = points.Length;
            for (int i = 0; i < points.Length; i++)
                renderer.SetPosition(i, new Vector3(points[i].x, points[i].y, 0f));

            renderer.startWidth = width;
            renderer.endWidth = width;
            renderer.startColor = _wallColor;
            renderer.endColor = _wallColor;
            renderer.textureMode = LineTextureMode.Tile;
            renderer.textureScale = Vector2.one;

            Material chainMaterial = _resources.CreateChainMaterialInstance();
            renderer.sharedMaterial = chainMaterial;
            chain.AddComponent<BossArenaChainTilingAnimator>().Initialize(chainMaterial);
            ApplySorting(renderer, 10);
        }

        private void CreateWand(string wandName, Vector2 localPosition, float size)
        {
            GameObject wand = new GameObject(wandName);
            wand.transform.SetParent(_parent.transform, false);
            wand.transform.localPosition = localPosition;
            wand.layer = _wallLayer;

            Sprite sprite = _resources.GetCornerSprite();
            SpriteRenderer renderer = wand.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = _wallColor;
            ApplySorting(renderer, 11);
            ScaleToSize(wand.transform, sprite, size);
        }

        private void ApplySorting(Renderer renderer, int sortingOrderOffset)
        {
            if (_background != null)
                renderer.sortingLayerID = _background.sortingLayerID;

            renderer.sortingOrder = _background != null ? _background.sortingOrder + sortingOrderOffset : sortingOrderOffset;
        }

        private static void ScaleToSize(Transform target, Sprite sprite, float size)
        {
            if (sprite == null)
                return;

            Vector2 spriteSize = sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
                return;

            target.localScale = new Vector3(size / spriteSize.x, size / spriteSize.y, 1f);
        }
    }
}
