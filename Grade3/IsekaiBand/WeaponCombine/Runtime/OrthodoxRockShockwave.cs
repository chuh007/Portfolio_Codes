using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class OrthodoxRockShockwave : MonoBehaviour
    {
        private readonly Collider2D[] _hits = new Collider2D[128];
        private readonly HashSet<int> _hitIds = new();
        private OrthodoxRockBandAttack _source;
        private float _maxRadius;
        private float _speed;
        private float _thickness;
        private float _radius;

        public void Init(OrthodoxRockBandAttack source, float maxRadius, float speed, float thickness)
        {
            _source = source;
            _maxRadius = maxRadius;
            _speed = speed;
            _thickness = thickness;
        }

        private void Update()
        {
            float previousRadius = _radius;
            _radius = Mathf.Min(_maxRadius, _radius + _speed * Time.deltaTime);

            int count = Physics2D.OverlapCircle(
                transform.position,
                _radius + _thickness,
                _source.TargetContactFilter,
                _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy") || !_hitIds.Add(hit.GetInstanceID())) continue;

                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < Mathf.Max(0f, previousRadius - _thickness)
                    || distance > _radius + _thickness)
                {
                    _hitIds.Remove(hit.GetInstanceID());
                    continue;
                }

                _source?.HitShockwave(hit, transform.position);
            }

            if (_radius >= _maxRadius)
                Destroy(gameObject);
        }
    }
}
