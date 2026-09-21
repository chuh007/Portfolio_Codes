using System.Collections.Generic;
using System.Threading;
using _Work.CHUH.Code.BT;
using _Work.CHUH.Code.Combat.Pattern;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class BossPhaseState
    {
        private const string LogPrefix = "[BossPhase]";
        private CancellationTokenSource _phaseCancellationCts = new();
        private BasePatternSO _forcedNextPattern;
        public CancellationToken CancellationToken => _phaseCancellationCts.Token;
        public bool IsTransitioning;
        public int PhaseIndex = -1;

        public BasePatternSO ConsumeForcedPattern()
        {
            BasePatternSO pattern = _forcedNextPattern;
            _forcedNextPattern = null;
            return pattern;
        }

        public void ForceNextPattern(Boss boss, BasePatternSO pattern)
        {
            if (pattern == null)
                return;

            _forcedNextPattern = pattern;
            boss.ChangeBtState(BTEnemyState.ATTACK_WARNING);
        }

        public void CancelCurrentPattern(Boss boss)
        {
            _forcedNextPattern = null;
            boss?.ClearPatternRecovery();
            _phaseCancellationCts.Cancel();
            _phaseCancellationCts.Dispose();
            _phaseCancellationCts = new();
        }

        public void Dispose()
        {
            _phaseCancellationCts.Cancel();
            _phaseCancellationCts.Dispose();
        }

        public void ApplyPhasePatterns(Boss boss, IReadOnlyList<BossPhaseData> phases, int phaseIndex)
        {
            IReadOnlyList<BossPhasePatternBinding> bindings = phases[phaseIndex].PatternBindings;
            for (int i = 0; i < bindings.Count; i++)
            {
                BossPhasePatternBinding binding = bindings[i];
                if (binding == null || binding.Pattern == null || string.IsNullOrWhiteSpace(binding.BlackboardVariableName))
                {
                    Debug.LogWarning(
                        $"{LogPrefix} Skip invalid pattern binding. phase={phaseIndex}, index={i}, " +
                        $"variable={(binding != null ? binding.BlackboardVariableName : "null")}, " +
                        $"pattern={(binding?.Pattern != null ? binding.Pattern.name : "null")}.",
                        boss);
                    continue;
                }

                if (!boss.SetBlackboardVariable(binding.BlackboardVariableName, binding.Pattern))
                {
                    Debug.LogWarning(
                        $"{LogPrefix} {boss.gameObject.name} could not set blackboard variable {binding.BlackboardVariableName}. " +
                        $"pattern={binding.Pattern.name}.",
                        boss);
                }
            }
        }

        public bool EnterPhaseImmediately(BossPhaseController source, int phaseIndex, bool startImmediatePattern)
        {
            if (phaseIndex < 0 || phaseIndex >= source.Phases.Count)
                return false;

            BossPhaseData phase = source.Phases[phaseIndex];
            source.CancelCurrentPattern();
            global::BossPatternSelection.ClearSelection(source.Boss);
            IsTransitioning = false;

            ApplyPhasePatterns(source.Boss, source.Phases, phaseIndex);
            PhaseIndex = phaseIndex;
            phase.OnPhaseEnter?.Invoke();
            source.RaisePhaseChanged(PhaseIndex);

            if (startImmediatePattern && phase.UseImmediatePatternOnEnter)
                source.ForceNextPattern(phase.ImmediatePattern);

            return true;
        }
    }
}
