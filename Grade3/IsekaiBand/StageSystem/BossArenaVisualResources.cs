using System.Collections.Generic;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaVisualResources
    {
        private readonly Sprite _cornerSprite;
        private readonly Material _chainMaterial;
        private Sprite _fallbackCornerSprite;
        private Material _fallbackChainMaterial;

        public BossArenaVisualResources(Sprite cornerSprite, Material chainMaterial)
        {
            _cornerSprite = cornerSprite;
            _chainMaterial = chainMaterial;
        }

        public Sprite GetCornerSprite()
        {
            if (_cornerSprite != null)
                return _cornerSprite;

            if (_fallbackCornerSprite != null)
                return _fallbackCornerSprite;

            _fallbackCornerSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);
            return _fallbackCornerSprite;
        }

        private Material GetChainMaterial()
        {
            if (_chainMaterial != null)
                return _chainMaterial;

            if (_fallbackChainMaterial != null)
                return _fallbackChainMaterial;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
                return null;

            _fallbackChainMaterial = new Material(shader);
            return _fallbackChainMaterial;
        }

        public Material CreateChainMaterialInstance()
        {
            Material source = GetChainMaterial();
            if (source == null)
                return null;

            Material instance = Object.Instantiate(source);
            instance.name = $"{source.name}_BossArenaInstance";
            return instance;
        }

        public void Dispose()
        {
            if (_fallbackCornerSprite != null)
                Object.Destroy(_fallbackCornerSprite);
            if (_fallbackChainMaterial != null)
                Object.Destroy(_fallbackChainMaterial);
            _fallbackCornerSprite = null;
            _fallbackChainMaterial = null;
        }
    }
}
