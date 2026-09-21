using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LaneSweepSequence
    {
        private readonly Phase2LaneSweepPatternSO _pattern;

        public LaneSweepSequence(Phase2LaneSweepPatternSO pattern) => _pattern = pattern;

        public async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            if (!_pattern.Plan.TryGetArenaBounds(out _pattern.Plan.ArenaBounds))
            {
                await _pattern.PrepareFallback(owner, ct);
                return;
            }

            _pattern.Plan.BuildDangerMasks();
            await _pattern.Presentation.HideOwner(owner, ct);
            if (ct.IsCancellationRequested)
            {
                await _pattern.Presentation.Cleanup(owner);
                return;
            }

            await _pattern.Camera.ZoomCameraToArena(ct);
            if (ct.IsCancellationRequested)
            {
                await _pattern.Presentation.Cleanup(owner);
                return;
            }

            await _pattern.Warnings.PlayWarnings(ct);

            if (ct.IsCancellationRequested)
                await _pattern.Presentation.Cleanup(owner);
        }

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null || _pattern.Plan.Sweeps.Count == 0)
            {
                await _pattern.Presentation.Cleanup(owner);
                return;
            }

            DamageData damage = _pattern.GetDamage(owner, _pattern.DamageMultiplier);
            CancellationToken executeCt = owner.destroyCancellationToken;

            for (int i = 0; i < _pattern.Plan.Sweeps.Count; i++)
            {
                if (ShouldStop(owner, executeCt))
                    break;

                float startTime = Time.time;
                await _pattern.Slashes.SweepDangerLanes(owner, _pattern.Plan.Sweeps[i], damage, executeCt);

                if (i < _pattern.Plan.Sweeps.Count - 1 && _pattern.SweepInterval > 0f)
                    await _pattern.Slashes.WaitUntilNextSweep(startTime, executeCt);
            }

            if (!ShouldStop(owner, executeCt) && _pattern.ReturnDelayAfterLastAttack > 0f)
                await UniTask.WaitForSeconds(_pattern.ReturnDelayAfterLastAttack, cancellationToken: executeCt)
                    .SuppressCancellationThrow();

            await _pattern.Presentation.Cleanup(owner);
        }

        public static bool ShouldStop(Enemy owner, CancellationToken ct)
        {
            return owner == null || owner.IsDead || ct.IsCancellationRequested;
        }
    }
}
