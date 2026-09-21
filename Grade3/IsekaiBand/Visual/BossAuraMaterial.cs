using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Visual
{
    internal class BossAuraMaterial
    {
        private Material _runtimeMaterial;
        private Texture2D _softParticleTexture;

        public Material GetOrCreateMaterial()
        {
            if (_runtimeMaterial != null)
                return _runtimeMaterial;

            Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null || !shader.isSupported)
                shader = Shader.Find("Sprites/Default");

            if (shader == null)
                return null;

            _runtimeMaterial = new Material(shader)
            {
                name = "Boss Aura Runtime Material",
                hideFlags = HideFlags.HideAndDontSave,
                mainTexture = GetOrCreateSoftParticleTexture()
            };
            return _runtimeMaterial;
        }

        private Texture2D GetOrCreateSoftParticleTexture()
        {
            if (_softParticleTexture != null)
                return _softParticleTexture;

            const int textureSize = 32;
            var pixels = new Color[textureSize * textureSize];
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float normalizedX = (x + 0.5f) / textureSize * 2f - 1f;
                    float normalizedY = (y + 0.5f) / textureSize * 2f - 1f;
                    float distance = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
                    float alpha = Mathf.Pow(Mathf.Clamp01(1f - distance), 1.6f);
                    pixels[y * textureSize + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            _softParticleTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false, true)
            {
                name = "Boss Aura Soft Particle",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };
            _softParticleTexture.SetPixels(pixels);
            _softParticleTexture.Apply(false, true);
            return _softParticleTexture;
        }

        public void Dispose()
        {
            if (_runtimeMaterial != null)
                Object.Destroy(_runtimeMaterial);

            if (_softParticleTexture != null)
                Object.Destroy(_softParticleTexture);
        }
    }
}
