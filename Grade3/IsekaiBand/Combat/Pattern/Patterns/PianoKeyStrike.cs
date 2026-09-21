using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoKeyStrike
    {
        private readonly PianoBossRuntime _runtime;
        private readonly Collider2D[] _hits = new Collider2D[48];

        public PianoKeyStrike(PianoBossRuntime runtime) => _runtime = runtime;

        public async UniTask StrikeKeysAsync(
            IReadOnlyList<int> keyIndices,
            float warningDuration,
            float activeDuration,
            DamageData damage,
            LayerMask targetMask,
            CancellationToken ct)
        {
            if (keyIndices == null || keyIndices.Count == 0)
                return;

            try
            {
                _runtime.FloorVisuals.HighlightKeys(keyIndices, _runtime.DangerKeyColor);
                if (warningDuration > 0f)
                    await UniTask.WaitForSeconds(warningDuration, cancellationToken: ct);

                _runtime.FloorVisuals.HighlightKeys(keyIndices, _runtime.ActiveKeyColor);
                for (int i = 0; i < keyIndices.Count; i++)
                    DamageKey(keyIndices[i], damage, targetMask);

                if (activeDuration > 0f)
                    await UniTask.WaitForSeconds(activeDuration, cancellationToken: ct);
            }
            finally
            {
                _runtime.FloorVisuals.ClearKeyHighlights(keyIndices);
            }
        }

        public async UniTask DropRhythmNoteAsync(
            int keyIndex,
            float fallDuration,
            DamageData damage,
            LayerMask targetMask,
            CancellationToken ct)
        {
            if (!_runtime.Floor.IsActive)
                return;

            int[] keys = { keyIndex };
            try
            {
                float duration = Mathf.Max(0.05f, fallDuration);
                _runtime.FloorVisuals.HighlightKeys(keys, _runtime.DangerKeyColor);
                await UniTask.WaitForSeconds(duration, cancellationToken: ct);

                _runtime.FloorVisuals.HighlightKeys(keys, _runtime.ActiveKeyColor);
                DamageKey(keyIndex, damage, targetMask);
                await UniTask.WaitForSeconds(0.08f, cancellationToken: ct);
            }
            finally
            {
                _runtime.FloorVisuals.ClearKeyHighlights(keys);
            }
        }

        public void DamageKey(int keyIndex, DamageData damage, LayerMask targetMask)
        {
            _runtime.Pose.KeySoundPlayer?.Play(keyIndex);
            Rect rect = _runtime.GetKeyRect(keyIndex);
            DamageRect(rect, damage, targetMask);
        }

        private void DamageRect(Rect rect, DamageData damage, LayerMask targetMask)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(targetMask);
            filter.useLayerMask = true;

            int count = Physics2D.OverlapBox(rect.center, rect.size, 0f, filter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null)
                    continue;

                IDamageable damageable = hit.GetComponent<IDamageable>() ?? hit.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(damage, Vector2.up, _runtime.Owner);
            }
        }
    }
}
