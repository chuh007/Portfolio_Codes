using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "PianoBossFloorPattern", menuName = "SO/Pattern/PianoBoss/Floor", order = 0)]
    public class PianoBossFloorPatternSO : PianoBossPatternBaseSO
    {
        [Header("Piano Floor")]
        [SerializeField, Min(0f)] private float floorDamageMultiplier = 0.58f;
        [SerializeField, Min(0.05f)] private float floorWarningDuration = 0.55f;
        [SerializeField, Min(0.01f)] private float floorActiveDuration = 0.1f;
        [SerializeField, Min(0.05f)] private float rhythmNoteInterval = 0.1f;

        protected override float FloorDamageMultiplier => floorDamageMultiplier;

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            PianoBossRuntime runtime = GetRuntime(owner);
            using (runtime.BeginPerformance())
            {
                EnsurePianoFloor(owner, runtime, CreateDamage(owner, FloorDamageMultiplier));
                await RunPianoFloorBurst(owner, runtime, ct);
            }
        }

        protected async UniTask RunPianoFloorBurst(Enemy owner, PianoBossRuntime runtime, CancellationToken ct)
        {
            DamageData damage = CreateDamage(owner, FloorDamageMultiplier);
            int variant = Random.Range(0, 4);
            switch (variant)
            {
                case 0:
                    await runtime.PlayTargetChordAsync(floorWarningDuration, floorActiveDuration, damage, whatIsTarget, ct);
                    break;

                case 1:
                    await runtime.PlayContiguousKeysAsync(Random.Range(3, 6), floorWarningDuration, floorActiveDuration, damage, whatIsTarget, ct);
                    break;

                case 2:
                    await runtime.PlayGlissandoAsync(
                        Random.value < 0.5f ? 1 : -1,
                        rhythmNoteInterval * 0.45f,
                        floorWarningDuration,
                        floorActiveDuration,
                        damage,
                        whatIsTarget,
                        ct);
                    break;

                default:
                    await runtime.PlayRandomSingleNotesAsync(8, rhythmNoteInterval, damage, whatIsTarget, ct);
                    break;
            }
        }
    }
}
