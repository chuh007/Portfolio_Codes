using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzBandAreaAttack
    {
        private const float BassZoneTickInterval = 0.2f;
        private readonly JazzBandAttack _source;
        private readonly System.Action<Collider2D, Vector3, float, float, float> _damageEnemy;
        private readonly Collider2D[] _hits = new Collider2D[192];
        private readonly HashSet<int> _hitTargetIds = new();

        public JazzBandAreaAttack(JazzBandAttack source,
            System.Action<Collider2D, Vector3, float, float, float> damageEnemy)
        {
            _source = source;
            _damageEnemy = damageEnemy;
        }

        public void SpawnDrumShockwave(
            Vector3 position, float damage, float radius, float knockback, bool pixelated)
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.DrumShockwave,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));

            DamageArea(position, radius, damage, knockback, false);

            Color color = new(1f, 0.72f, 0.18f, 0.66f);
            if (!DrumWaveAnimationEffect.Spawn(position, radius, color, 0.38f, 54))
            {
                BuildVisualEffect.SpawnCircle(
                    position, radius, color, 0.38f, 54, pixelated);
            }

            _source.RaiseImpact(new AttackEventContext(
                _source, null, position, Vector2.zero, damage, radius, radius,
                0f, 0f, 0, 0f, knockback));
        }

        public void DamageArea(
            Vector3 position, float radius, float damage, float knockback, bool applySlow)
        {
            _hitTargetIds.Clear();
            int count = Physics2D.OverlapCircle(
                position,
                radius,
                _source.TargetContactFilter,
                _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = GetTargetId(hit);
                if (!_hitTargetIds.Add(targetId))
                    continue;

                _damageEnemy(hit, position, damage, radius, knockback);
                if (!applySlow)
                    continue;

                ISlowable slowable = hit.GetComponent<ISlowable>()
                                     ?? hit.GetComponentInParent<ISlowable>();
                slowable?.ApplySlow(0.7f, _source.ScaleCommonSlowDuration(BassZoneTickInterval + 0.1f));
            }
        }

        private static int GetTargetId(Collider2D hit)
        {
            Entity entity = hit.GetComponent<Entity>() ?? hit.GetComponentInParent<Entity>();
            return entity != null
                ? entity.GetInstanceID()
                : hit.transform.root.GetInstanceID();
        }
    }
}
