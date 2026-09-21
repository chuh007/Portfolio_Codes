using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class JazzDamageZone : MonoBehaviour
    {
        private static Sprite _zoneSprite;
        private JazzDuoAttack _source;
        private SpriteRenderer _renderer;
        private float _radius;
        private float _lifetime;
        private float _damageInterval;
        private float _age;
        private float _damageTimer;

        public void Init(
            JazzDuoAttack source,
            float radius,
            float lifetime,
            float damageInterval)
        {
            _source = source;
            _radius = Mathf.Max(0.1f, radius);
            _lifetime = Mathf.Max(0.1f, lifetime);
            _damageInterval = Mathf.Max(0.05f, damageInterval);

            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = ZoneSprite;
            GroundEffectRenderLayer.ApplyTo(_renderer, 3);
            UpdateVisual();
        }

        private void Update()
        {
            _age += Time.deltaTime;
            _damageTimer += Time.deltaTime;
            float interval = _source != null ? _source.ScaleCommonInterval(_damageInterval) : _damageInterval;
            while (_damageTimer >= interval)
            {
                _damageTimer -= interval;
                _source?.DamageZone(transform.position, _radius);
            }

            UpdateVisual();
            if (_age >= _lifetime)
                Destroy(gameObject);
        }

        private void UpdateVisual()
        {
            float normalizedAge = Mathf.Clamp01(_age / _lifetime);
            float pulse = (Mathf.Sin(_age * Mathf.PI * 4f) + 1f) * 0.5f;
            float scale = _radius * 2f * Mathf.Lerp(0.96f, 1.04f, pulse);
            transform.localScale = new Vector3(scale, scale, 1f);

            if (_renderer == null) return;

            _renderer.color = new Color(
                0.3f,
                0.95f,
                0.78f,
                Mathf.Lerp(0.42f, 0f, normalizedAge));
        }

        private static Sprite ZoneSprite
        {
            get
            {
                if (_zoneSprite != null) return _zoneSprite;

                const int size = 64;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Bilinear
                };
                Vector2 center = Vector2.one * ((size - 1) * 0.5f);
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                        float alpha = distance <= 1f ? Mathf.Lerp(0.18f, 0.75f, distance) : 0f;
                        texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                }
                texture.Apply();
                _zoneSprite = Sprite.Create(
                    texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
                return _zoneSprite;
            }
        }
    }
}
