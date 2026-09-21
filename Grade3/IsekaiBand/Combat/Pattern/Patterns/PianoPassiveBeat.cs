using System;
using System.Threading;
using _Work.CHUH.Code.Enemies.Boss;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class PianoPassiveBeat
    {
        private readonly PianoBossRuntime _runtime;
        private float _passiveBeatTimer;
        private CancellationTokenSource _passivePatternCts;
        private bool _passivePatternRunning;
        public PianoPassiveBeat(PianoBossRuntime runtime) => _runtime = runtime;

        public void UpdatePianoFloor()
        {
            if (_runtime.Pose.Boss != null && !_runtime.Pose.Boss.IsPatternExecutionEnabled)
            {
                StopPassivePattern();
                ResetPassiveBeatTimer();
                return;
            }

            if (!_runtime.Floor.IsActive || _runtime.FloorVisuals.Root == null || _passivePatternRunning)
                return;

            if (_runtime.Owner is Boss boss && boss.CurrentPhaseIndex < 1)
                return;

            if (_runtime.Performance.IsActive)
            {
                ResetPassiveBeatTimer();
                return;
            }

            _passiveBeatTimer -= Time.deltaTime;
            if (_passiveBeatTimer > 0f)
                return;

            _passiveBeatTimer = _runtime.PassiveBeatInterval;
            StartPassivePattern();
        }

        private void StartPassivePattern()
        {
            StopPassivePattern();
            _passivePatternCts = CreatePassivePatternCts();
            PlayPassiveBeatAsync(_passivePatternCts).Forget();
        }

        private CancellationTokenSource CreatePassivePatternCts()
        {
            if (_runtime.Owner is Boss boss)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(
                    _runtime.Owner.destroyCancellationToken,
                    boss.PhaseCancellationToken);
            }

            return CancellationTokenSource.CreateLinkedTokenSource(_runtime.Owner.destroyCancellationToken);
        }

        public void StopPassivePattern()
        {
            if (_passivePatternCts == null)
                return;

            _passivePatternCts.Cancel();
            _passivePatternCts = null;
            _passivePatternRunning = false;
        }

        public void ResetPassiveBeatTimer()
        {
            _passiveBeatTimer = Mathf.Max(_passiveBeatTimer, _runtime.PassiveBeatInterval);
        }

        private async UniTaskVoid PlayPassiveBeatAsync(CancellationTokenSource cts)
        {
            CancellationToken ct = cts.Token;
            _passivePatternRunning = true;
            try
            {
                await _runtime.FloorPatterns.PlayRandomBeatAsync(ct);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_passivePatternCts == cts)
                {
                    _passivePatternCts = null;
                    _passivePatternRunning = false;
                }

                cts.Dispose();
            }
        }
    }
}
