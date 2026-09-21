using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class GrandPianoImpact
    {
        private readonly PianoBossHumanGrandPianoDropPatternSO _pattern;
        private readonly Collider2D[] _hits = new Collider2D[32];
        private readonly HashSet<IDamageable> _damagedTargets = new();
        public GrandPianoImpact(PianoBossHumanGrandPianoDropPatternSO pattern) => _pattern = pattern;

        public void PlayImpactAudio(Enemy owner)
        {
            if (!string.IsNullOrWhiteSpace(_pattern.ImpactSoundKey))
            {
                Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                    _pattern.ImpactSoundKey,
                    SoundType.SFX));
            }

            owner?.GetComponent<PianoKeySoundPlayer>()
                ?.PlayRandomChord(_pattern.RandomImpactNoteCount);
        }

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
            ContactFilter2D filter = new();
            filter.SetLayerMask(_pattern.TargetMask);
            filter.useLayerMask = true;

            int count = Physics2D.OverlapCircle(position, _pattern.ImpactRadius, filter, _hits);
            Vector2 direction = owner != null
                ? (position - (Vector2)owner.transform.position).normalized
                : Vector2.down;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
                direction = Vector2.down;

            _damagedTargets.Clear();
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null)
                    continue;

                IDamageable damageable = hit.GetComponent<IDamageable>()
                                        ?? hit.GetComponentInParent<IDamageable>();
                if (damageable != null && _damagedTargets.Add(damageable))
                    damageable.TakeDamage(damage, direction, owner);
            }
        }
    }
}
