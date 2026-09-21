using System.Collections.Generic;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaChainTilingAnimator : MonoBehaviour
    {
        private const float MinTilingX = -0.5f;
        private const float MaxTilingX = 0.5f;
        private const float FixedTilingY = 1f;
        private const float FixedOffsetY = 0f;
        private const float UpdateInterval = 0.1f;

        private Material _material;
        private float _elapsed;

        public void Initialize(Material material)
        {
            _material = material;
            ApplyRandomTiling();
        }

        private void OnDestroy()
        {
            if (_material != null)
                Object.Destroy(_material);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed < UpdateInterval)
                return;

            _elapsed = 0f;
            ApplyRandomTiling();
        }

        private void ApplyRandomTiling()
        {
            if (_material == null)
                return;

            _material.mainTextureScale = new Vector2(Random.Range(MinTilingX, MaxTilingX), FixedTilingY);
            _material.mainTextureOffset = new Vector2(_material.mainTextureOffset.x, FixedOffsetY);
        }
    }
}
