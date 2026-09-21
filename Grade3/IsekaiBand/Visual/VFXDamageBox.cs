using System.Collections;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using Chuh007Lib.Entities.Entities;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Visual
{
    public class VFXDamageBox : MonoBehaviour
    {
        [SerializeField] private Vector2 hitBoxSize = new Vector2(5.5f, 4f);
        [SerializeField] private Vector2 hitBoxOffset;
        [SerializeField] private ContactFilter2D whatIsTarget;

        private readonly VfxDamageQuery _query = new();
        private RepeatedVfxHit _repeatedHit;
        private RepeatedVfxHit RepeatedHit => _repeatedHit ??= new(this);

        public void Hit(DamageData damage, Vector2 direction, Entity dealer)
            => HitAndGetCount(damage, direction, dealer, GetScaledSize(hitBoxSize));

        public void Hit(DamageData damage, Vector2 direction, Entity dealer, Vector2 worldSize)
            => HitAndGetCount(damage, direction, dealer, worldSize);

        public int HitAndGetCount(DamageData damage, Vector2 direction, Entity dealer, Vector2 worldSize)
            => _query.HitAndGetCount(transform, hitBoxOffset, whatIsTarget, damage, direction, dealer, worldSize);

        public int HitCircleAndGetCount(DamageData damage, Vector2 direction, Entity dealer, float radius)
            => _query.HitCircleAndGetCount(transform, hitBoxOffset, whatIsTarget, damage, direction, dealer, radius);

        public int HitRepeatedAndGetCount(DamageData damage, Vector2 direction, Entity dealer,
            Vector2 worldSize, float interval)
            => RepeatedHit.Start(() => HitAndGetCount(damage, direction, dealer, worldSize), interval);

        public int HitCircleRepeatedAndGetCount(DamageData damage, Vector2 direction, Entity dealer,
            float radius, float interval)
            => RepeatedHit.Start(() => HitCircleAndGetCount(damage, direction, dealer, radius), interval);

        private void OnDisable() => _repeatedHit?.Stop();

        private Vector2 GetScaledSize(Vector2 size)
        {
            Vector2 scale = transform.lossyScale;
            return new Vector2(Mathf.Abs(size.x * scale.x), Mathf.Abs(size.y * scale.y));
        }
    }
}
