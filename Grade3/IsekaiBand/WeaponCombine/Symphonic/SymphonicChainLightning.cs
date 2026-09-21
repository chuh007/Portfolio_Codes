using System.Collections.Generic;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class SymphonicChainLightning
    {
        private const float ChainDamageMultiplier = 0.55f;
        private readonly SymphonicRockAttack _source;
        private readonly SymphonicRockStats _stats;
        private readonly System.Action<Collider2D, Vector3, float, float, float> _damageEnemy;
        private readonly Collider2D[] _chainHits = new Collider2D[64];

        public SymphonicChainLightning(SymphonicRockAttack source, SymphonicRockStats stats,
            System.Action<Collider2D, Vector3, float, float, float> damageEnemy)
        {
            _source = source;
            _stats = stats;
            _damageEnemy = damageEnemy;
        }

        public void TriggerChainLightning(Collider2D originTarget)
        {
            if (originTarget == null)
                return;

            Vector3 origin = originTarget.transform.position;
            int originTargetId = BandRuntimeVisuals.GetTargetId(originTarget);
            int count = Physics2D.OverlapCircle(
                origin,
                _source.ScaleCommonRange(_stats.BounceRange),
                _source.TargetContactFilter,
                _chainHits);
            Collider2D nearest = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider2D candidate = _chainHits[i];
                if (candidate == null || !candidate.CompareTag("Enemy"))
                    continue;
                if (!ManualTargetingService.IsValid(candidate.transform))
                    continue;
                if (BandRuntimeVisuals.GetTargetId(candidate) == originTargetId)
                    continue;

                float distance = ((Vector2)candidate.transform.position - (Vector2)origin)
                    .sqrMagnitude;
                if (distance >= nearestDistance)
                    continue;
                nearestDistance = distance;
                nearest = candidate;
            }

            if (nearest == null)
                return;

            _source.AttackAudio.Play(SoundKeys.ElectricProjectileDischarge);
            BuildVisualEffect.SpawnLine(
                origin,
                nearest.transform.position,
                new Color(0.62f, 0.94f, 1f, 0.82f),
                0.16f,
                0.24f,
                55);
            _damageEnemy(
                nearest,
                origin,
                _stats.Damage * ChainDamageMultiplier,
                _source.ScaleCommonRange(_stats.BounceRange), 0f);
        }
    }
}
