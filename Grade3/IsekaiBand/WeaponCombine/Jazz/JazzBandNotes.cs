using _Code.LCH._02.Scripts.Core;
using _Work.CHUH.Code.Audio;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzBandNotes
    {
        private const int NotePierceCount = 1;
        private const float TargetedFanAngle = 130f;
        private const float NoteHomingStrength = 0.42f;
        private const float NoteVisualScale = 1.25f;
        private readonly JazzBandAttack _source;
        private readonly JazzBandStats _stats;
        private readonly BandRuntimeObjects _notes;
        private readonly System.Action<KeyboardProjectileImpact, bool> _handleImpact;
        private int _noteIndex;

        public JazzBandNotes(JazzBandAttack source, JazzBandStats stats, BandRuntimeObjects notes,
            System.Action<KeyboardProjectileImpact, bool> handleImpact)
        {
            _source = source;
            _stats = stats;
            _notes = notes;
            _handleImpact = handleImpact;
        }

        public void FireTargetedFan(Transform target, int noteCount)
        {
            Vector2 targetDirection = ((Vector2)target.position - (Vector2)_source.Position).normalized;
            if (targetDirection.sqrMagnitude < 0.001f)
                targetDirection = Vector2.right;

            float centerAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            for (int i = 0; i < noteCount; i++)
            {
                float normalized = noteCount > 1 ? i / (float)(noteCount - 1) : 0.5f;
                float offset = Mathf.Lerp(
                    -TargetedFanAngle * 0.5f,
                    TargetedFanAngle * 0.5f,
                    normalized);
                float angle = centerAngle + offset + Random.Range(-4f, 4f);
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                SpawnPianoNote(direction);
            }
        }

        public void FireRadialNotes(int noteCount)
        {
            float sectorAngle = 360f / noteCount;
            float startAngle = Random.Range(0f, sectorAngle);
            for (int i = 0; i < noteCount; i++)
            {
                float angle = startAngle + sectorAngle * i;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                SpawnPianoNote(direction);
            }
        }

        private void SpawnPianoNote(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 origin = _source.Position + (Vector3)(direction * 0.32f);
            GameObject obj = ProjectilePool.Pop(
                "PianoNoteProjectile",
                origin,
                Quaternion.Euler(0f, 0f, angle));
            if (obj == null)
                return;

            obj.name = "JazzBandHomingNote";
            obj.transform.localScale *= NoteVisualScale;
            ProjectileRenderLayer.ApplyTo(obj);

            if (!obj.TryGetComponent<KeyboardProjectile>(out KeyboardProjectile projectile))
            {
                ProjectilePool.Push(obj);
                return;
            }

            bool spawnedBassZone = false;
            projectile.Init(new KeyboardProjectileSettings
            {
                TargetLayerMask = _source.TargetLayerMask,
                Speed = _stats.NoteSpeed,
                MaxRange = _source.ScaleCommonRange(_stats.NoteRange),
                Damage = _source.ScaleCommonDamage(_stats.NoteDamage),
                NoteIndex = _noteIndex++,
                IsPiercing = true,
                PierceCountRemaining = NotePierceCount,
                HomingNotes = true,
                HomingStrength = NoteHomingStrength,
                EventSource = _source,
                ImpactEffect = impact =>
                {
                    _handleImpact(impact, !spawnedBassZone);
                    spawnedBassZone = true;
                },
                ReturnedToPool = OnNoteReturned
            });
            _notes.Add(obj);
            _source.AttackAudio.Play(SoundKeys.PianoAttack);
        }

        private void OnNoteReturned(KeyboardProjectile projectile)
        {
            if (projectile != null)
                _notes.Remove(projectile.gameObject);
        }
    }
}
