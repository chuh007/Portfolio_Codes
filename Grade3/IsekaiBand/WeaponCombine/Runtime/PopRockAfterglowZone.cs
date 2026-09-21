using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class PopRockAfterglowZone : MonoBehaviour
    {
        private PopRockBandAttack _source;
        private float _duration;
        private float _tickInterval;
        private float _elapsed;
        private float _tickTimer;

        public void Init(
            PopRockBandAttack source,
            float duration,
            float tickInterval,
            float radius)
        {
            _source = source;
            _duration = Mathf.Max(0.05f, duration);
            _tickInterval = Mathf.Max(0.05f, tickInterval);

            BuildVisualEffect.SpawnCircle(
                transform.position,
                Mathf.Max(0.1f, radius),
                new Color(0.42f, 0.92f, 1f, 0.24f),
                _tickInterval + 0.08f,
                1,
                sortingLayerName: GroundEffectRenderLayer.SortingLayerName);
        }

        private void Update()
        {
            if (_source == null)
            {
                Destroy(gameObject);
                return;
            }

            float deltaTime = Time.deltaTime;
            _elapsed += deltaTime;
            _tickTimer += deltaTime;

            float interval = _source.ScaleCommonInterval(_tickInterval);
            while (_tickTimer >= interval && _elapsed < _duration)
            {
                _tickTimer -= interval;
                _source.TickAfterglow(transform.position);
            }

            if (_elapsed >= _duration)
                Destroy(gameObject);
        }
    }
}
