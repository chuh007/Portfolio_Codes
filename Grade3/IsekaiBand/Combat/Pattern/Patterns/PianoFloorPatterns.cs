using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoFloorPatterns
    {
        private readonly PianoBossRuntime _runtime;
        private static readonly List<int> s_IntBuffer = new();
        public PianoFloorPatterns(PianoBossRuntime runtime) => _runtime = runtime;

        public async UniTask PlayTargetChordAsync(
            float warningDuration,
            float activeDuration,
            DamageData damage,
            LayerMask targetMask,
            CancellationToken ct)
        {
            int center = _runtime.Owner != null && _runtime.Owner.target != null ? _runtime.GetKeyIndex(_runtime.Owner.target.transform.position) : _runtime.KeyCount / 2;
            int[] chord = BuildUniqueKeys(center - 2, center, center + 2);
            await _runtime.StrikeKeysAsync(chord, warningDuration, activeDuration, damage, targetMask, ct);
        }

        public async UniTask PlayContiguousKeysAsync(
            int count,
            float warningDuration,
            float activeDuration,
            DamageData damage,
            LayerMask targetMask,
            CancellationToken ct)
        {
            int safeCount = Mathf.Clamp(count, 1, _runtime.KeyCount);
            int center = _runtime.Owner != null && _runtime.Owner.target != null ? _runtime.GetKeyIndex(_runtime.Owner.target.transform.position) : _runtime.KeyCount / 2;
            int start = Mathf.Clamp(center - safeCount / 2, 0, _runtime.KeyCount - safeCount);
            int[] keys = new int[safeCount];
            for (int i = 0; i < safeCount; i++)
                keys[i] = start + i;

            await _runtime.StrikeKeysAsync(keys, warningDuration, activeDuration, damage, targetMask, ct);
        }

        public async UniTask PlayRandomSingleNotesAsync(
            int count,
            float interval,
            DamageData damage,
            LayerMask targetMask,
            CancellationToken ct)
        {
            int total = Mathf.Max(1, count);
            int targetKey = _runtime.Owner != null && _runtime.Owner.target != null ? _runtime.GetKeyIndex(_runtime.Owner.target.transform.position) : Random.Range(0, _runtime.KeyCount);
            var noteTasks = new List<UniTask>(total);

            for (int i = 0; i < total; i++)
            {
                int key = i == total / 2 ? targetKey : Random.Range(0, _runtime.KeyCount);
                noteTasks.Add(_runtime.DropRhythmNoteAsync(key, 0.55f, damage, targetMask, ct));

                if (interval > 0f && i < total - 1)
                    await UniTask.WaitForSeconds(interval, cancellationToken: ct);
            }

            await UniTask.WhenAll(noteTasks);
        }

        private int[] BuildUniqueKeys(params int[] rawKeys)
        {
            s_IntBuffer.Clear();
            for (int i = 0; i < rawKeys.Length; i++)
            {
                int key = Mathf.Clamp(rawKeys[i], 0, _runtime.KeyCount - 1);
                if (!s_IntBuffer.Contains(key))
                    s_IntBuffer.Add(key);
            }

            return s_IntBuffer.ToArray();
        }

        public async UniTask PlayRandomBeatAsync(CancellationToken ct)
        {
            int variant = Random.Range(0, 4);
            switch (variant)
            {
                case 0:
                    await PlayTargetChordAsync(0.68f, 0.1f, _runtime.Floor.Damage, _runtime.Floor.TargetMask, ct);
                    break;

                case 1:
                    await PlayContiguousKeysAsync(Random.Range(3, 6), 0.66f, 0.1f, _runtime.Floor.Damage, _runtime.Floor.TargetMask, ct);
                    break;

                case 2:
                    await _runtime.PlayGlissandoAsync(Random.value < 0.5f ? 1 : -1, 0.055f, 0.58f, 0.05f, _runtime.Floor.Damage, _runtime.Floor.TargetMask, ct);
                    break;

                default:
                    await PlayRandomSingleNotesAsync(8, 0.16f, _runtime.Floor.Damage, _runtime.Floor.TargetMask, ct);
                    break;
            }
        }
    }
}
