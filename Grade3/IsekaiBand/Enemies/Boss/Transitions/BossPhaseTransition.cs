using _Work.CHUH.Code.BT;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class BossPhaseTransition
    {
        private readonly BossPhaseController _source;
        private readonly BossPhaseState _state;

        public BossPhaseTransition(BossPhaseController source, BossPhaseState state)
        {
            _source = source;
            _state = state;
        }

        public void HandlePhaseHealthChanged(float currentHealth)
        {
            if (_source.Boss.IsDead || _state.IsTransitioning || currentHealth <= 0f || _source.Health == null)
                return;

            for (int i = _state.PhaseIndex + 1; i < _source.Phases.Count; i++)
            {
                if (!IsPhaseTriggered(_source.Phases[i], currentHealth))
                    continue;

                TransitionToPhaseAsync(i).Forget();
                return;
            }
        }

        private bool IsPhaseTriggered(BossPhaseData phase, float currentHealth)
        {
            if (_source.Health.MaxHealth <= 0f)
                return false;

            if (phase.TriggerType == BossPhaseTriggerType.HealthPercent)
                return currentHealth / _source.Health.MaxHealth <= phase.HealthPercentThreshold;

            if (phase.TriggerType == BossPhaseTriggerType.LostHealthLines)
            {
                float lineHealth = _source.Health.MaxHealth / Mathf.Max(1, _source.HealthLineCount);
                int lostHealthLines = Mathf.FloorToInt((_source.Health.MaxHealth - currentHealth) / lineHealth);
                return lostHealthLines >= phase.LostHealthLinesThreshold;
            }

            return false;
        }

        public async UniTaskVoid TransitionToPhaseAsync(int phaseIndex)
        {
            if (phaseIndex <= _state.PhaseIndex || phaseIndex >= _source.Phases.Count || _state.IsTransitioning)
                return;

            BossPhaseData phase = _source.Phases[phaseIndex];
            EntityMover mover = _source.Boss.GetCompo<EntityMover>();
            bool previousCanManualMove = mover != null && mover.CanManualMove;

            _state.IsTransitioning = true;
            _source.CancelCurrentPattern();
            global::BossPatternSelection.ClearSelection(_source.Boss);

            try
            {
                if (phase.ForceWaitStateOnTransition)
                    _source.Boss.ChangeBtState(BTEnemyState.WAIT);

                if (phase.StopMoveDuringTransition && mover != null)
                {
                    mover.StopImmediately();
                    mover.CanManualMove = false;
                }

                phase.OnTransitionStart?.Invoke();

                if (_source.TransitionPresentation != null &&
                    await _source.TransitionPresentation.PlayAsync(_source.Boss, mover, _source.Boss.destroyCancellationToken))
                    return;

                if (phase.TransitionDelay > 0f)
                {
                    bool canceled = await UniTask.WaitForSeconds(
                        phase.TransitionDelay,
                        cancellationToken: _source.Boss.destroyCancellationToken).SuppressCancellationThrow();
                    if (canceled)
                        return;
                }

                _state.ApplyPhasePatterns(_source.Boss, _source.Phases, phaseIndex);
                _state.PhaseIndex = phaseIndex;
                phase.OnPhaseEnter?.Invoke();
                _source.RaisePhaseChanged(_state.PhaseIndex);
                phase.OnTransitionEnd?.Invoke();
            }
            finally
            {
                if (phase.StopMoveDuringTransition && mover != null)
                    mover.CanManualMove = previousCanManualMove;

                _state.IsTransitioning = false;
            }

            if (phase.UseImmediatePatternOnEnter)
                _source.ForceNextPattern(phase.ImmediatePattern);

            if (_source.EvaluateSkippedPhase && _source.Health != null)
                HandlePhaseHealthChanged(_source.Health.CurrentHealth);
        }
    }
}
