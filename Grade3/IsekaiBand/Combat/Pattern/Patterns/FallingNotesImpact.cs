using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class FallingNotesImpact
    {
        private readonly PianoBossHumanFallingNotesPatternSO _pattern;
        private readonly Collider2D[] _hits = new Collider2D[32];
        private readonly HashSet<IDamageable> _damagedTargets = new();
        public FallingNotesImpact(PianoBossHumanFallingNotesPatternSO pattern) => _pattern = pattern;

        public void PlayImpactShockwave(Vector2 position)
        {
            if (DrumWaveAnimationEffect.Spawn(
                    position,
                    _pattern.ShockwaveRadius,
                    _pattern.ShockwaveColor,
                    _pattern.ShockwaveDuration,
                    _pattern.ShockwaveSortingOrder))
                return;

            BuildVisualEffect.SpawnCircle(
                position,
                _pattern.ShockwaveRadius,
                _pattern.ShockwaveColor,
                _pattern.ShockwaveDuration,
                _pattern.ShockwaveSortingOrder,
                true,
                "Projectile");
        }

        public void DamageTargets(Enemy owner, Vector2 position, DamageData damage)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(_pattern.TargetMask);
            filter.useLayerMask = true;

            int count = Physics2D.OverlapCircle(position, _pattern.ImpactRadius, filter, _hits);
            Vector2 direction = owner != null
                ? (position - (Vector2)owner.transform.position).normalized
                : Vector2.down;

            _damagedTargets.Clear();
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null)
                    continue;

                IDamageable damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();
                if (damageable != null && _damagedTargets.Add(damageable))
                    damageable.TakeDamage(damage, direction, owner);
            }
        }
    }
}
