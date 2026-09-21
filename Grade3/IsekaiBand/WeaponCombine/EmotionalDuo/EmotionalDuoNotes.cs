using System.Collections.Generic;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.Data;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class EmotionalDuoNotes
    {
        private const int NotesPerVolley = 3;
        private readonly EmotionalDuoAttack _source;
        private readonly EmotionalDuoStats _stats;
        private int _noteIndex;

        public EmotionalDuoNotes(EmotionalDuoAttack source, EmotionalDuoStats stats)
        {
            _source = source;
            _stats = stats;
        }

        public void FireVolley(Vector2 facing)
        {
            int noteCount = _source.CountProjectiles(NotesPerVolley);
            for (int i = 0; i < noteCount; i++)
                FireRandomNote(facing);
        }

        private void FireRandomNote(Vector2 facing)
        {
            float offset = Random.Range(-_stats.ConeAngle * 0.5f, _stats.ConeAngle * 0.5f);
            Vector2 direction = Quaternion.Euler(0f, 0f, offset) * facing.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject obj = ProjectilePool.Pop(
                "PianoNoteProjectile",
                _source.Position,
                Quaternion.Euler(0f, 0f, angle));
            if (obj == null) return;

            if (!obj.TryGetComponent<KeyboardProjectile>(out var projectile))
            {
                ProjectilePool.Push(obj);
                return;
            }

            projectile.Init(new KeyboardProjectileSettings
            {
                TargetLayerMask = _source.TargetLayerMask,
                Speed = _stats.NoteSpeed,
                MaxRange = _source.ScaleCommonRange(_stats.VocalRange),
                Damage = _source.ScaleCommonDamage(_stats.NoteDamage),
                NoteIndex = _noteIndex++,
                EventSource = _source
            });
            _source.AttackAudio.Play(SoundKeys.PianoAttack);

            BuildVisualEffect.SpawnCircle(
                _source.Position,
                0.55f,
                _noteIndex % 2 == 0
                    ? new Color(0.35f, 0.92f, 1f, 0.58f)
                    : new Color(1f, 0.42f, 0.72f, 0.58f),
                0.2f,
                52,
                true);
        }
    }
}
