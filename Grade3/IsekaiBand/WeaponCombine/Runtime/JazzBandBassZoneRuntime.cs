using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class JazzBandBassZoneRuntime : MonoBehaviour
    {
        private JazzBandAttack _source;
        private float _duration;
        private float _tickInterval;
        private float _elapsed;
        private float _tickTimer;

        public void Init(JazzBandAttack source, float duration, float tickInterval)
        {
            _source = source;
            _duration = Mathf.Max(0.05f, duration);
            _tickInterval = Mathf.Max(0.05f, tickInterval);
            _source?.TickBassZone(transform.position);
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
                _source.TickBassZone(transform.position);
            }

            if (_elapsed >= _duration)
                Destroy(gameObject);
        }
    }
}
