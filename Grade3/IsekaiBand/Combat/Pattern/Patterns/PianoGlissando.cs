using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoGlissando
    {
        private readonly PianoBossRuntime _runtime;


        public PianoGlissando(PianoBossRuntime runtime) => _runtime = runtime;

        public async UniTask PlayGlissandoAsync(
            int direction,
            float interval,
            float warningHoldDuration,
            float activeDuration,
            DamageData damage,
            LayerMask targetMask,
            CancellationToken ct)
        {
            int start = direction >= 0 ? 0 : _runtime.KeyCount - 1;
            int step = direction >= 0 ? 1 : -1;
            int[] keys = new int[_runtime.KeyCount];
            for (int i = 0; i < keys.Length; i++)
                keys[i] = start + i * step;

            float speedMultiplier = Mathf.Max(0.1f, _runtime.OrderedKeyPatternSpeedMultiplier);
            float stepInterval = Mathf.Max(0f, interval) / speedMultiplier;
            float warningHold = Mathf.Max(0f, warningHoldDuration);
            float hitDuration = Mathf.Max(0.02f, activeDuration) / speedMultiplier;

            try
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    _runtime.FloorVisuals.HighlightKeys(new[] { keys[i] }, _runtime.DangerKeyColor);
                    if (stepInterval > 0f)
                        await UniTask.WaitForSeconds(stepInterval, cancellationToken: ct);
                }

                if (warningHold > 0f)
                    await UniTask.WaitForSeconds(warningHold, cancellationToken: ct);

                for (int i = 0; i < keys.Length; i++)
                {
                    int key = keys[i];
                    _runtime.FloorVisuals.HighlightKeys(new[] { key }, _runtime.ActiveKeyColor);
                    _runtime.KeyStrike.DamageKey(key, damage, targetMask);
                    await UniTask.WaitForSeconds(hitDuration, cancellationToken: ct);
                    _runtime.FloorVisuals.ClearKeyHighlights(new[] { key });

                    if (stepInterval > 0f)
                        await UniTask.WaitForSeconds(stepInterval, cancellationToken: ct);
                }
            }
            finally
            {
                _runtime.FloorVisuals.ClearKeyHighlights(keys);
            }
        }
    }
}
