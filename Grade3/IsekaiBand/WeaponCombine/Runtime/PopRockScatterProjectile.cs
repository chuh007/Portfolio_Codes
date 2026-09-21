using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class PopRockScatterProjectile : MonoBehaviour
    {
        private PopRockBandAttack _source;
        private Vector2 _direction;
        private Vector3 _origin;
        private Vector3 _baseScale;
        private float _travelDistance;
        private float _decelerationDuration;
        private float _explosionDelay;
        private float _elapsed;

        public void Init(
            PopRockBandAttack source,
            Vector2 direction,
            float travelDistance,
            float decelerationDuration,
            float explosionDelay)
        {
            enabled = true;
            _elapsed = 0f;
            _source = source;
            _direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
            _origin = transform.position;
            _baseScale = transform.localScale;
            _travelDistance = Mathf.Max(0f, travelDistance);
            _decelerationDuration = Mathf.Max(0.01f, decelerationDuration);
            _explosionDelay = Mathf.Max(0f, explosionDelay);
        }

        private void Update()
        {
            if (_source == null)
            {
                ReturnToPool(true);
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed < _decelerationDuration)
            {
                float t = Mathf.Clamp01(_elapsed / _decelerationDuration);
                float easedDistance = 1f - Mathf.Pow(1f - t, 3f);
                transform.position = _origin + (Vector3)(_direction * (_travelDistance * easedDistance));
                transform.Rotate(0f, 0f, 300f * Time.deltaTime);
                return;
            }

            transform.position = _origin + (Vector3)(_direction * _travelDistance);
            float waitProgress = _explosionDelay > 0f
                ? Mathf.Clamp01((_elapsed - _decelerationDuration) / _explosionDelay)
                : 1f;
            float pulse = 1f + Mathf.Sin(waitProgress * Mathf.PI * 6f) * 0.12f + waitProgress * 0.2f;
            transform.localScale = _baseScale * pulse;

            if (_elapsed < _decelerationDuration + _explosionDelay)
                return;

            _source.Explode(this, transform.position);
            ReturnToPool(false);
        }

        private void ReturnToPool(bool notifySource)
        {
            PopRockBandAttack source = _source;
            _source = null;
            if (notifySource)
                source?.ProjectileEnded(this);
            ProjectilePool.Push(gameObject);
        }
    }
}
