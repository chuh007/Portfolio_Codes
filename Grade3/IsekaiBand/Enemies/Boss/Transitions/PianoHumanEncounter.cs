using _Work.CHUH.Code.Enemies.AttackCompo;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class PianoHumanEncounter
    {
        private readonly PianoBossHumanFormTransition _source;
        private readonly PianoHumanEncounterIntro _intro;
        private readonly PianoHumanFormChange _change;
        private bool _transitioning;
        private bool _encounterActive;
        public PianoHumanBody Body { get; } = new();
        public int RewardCount { get; private set; }
        public bool IsTransitioning => _transitioning;
        public bool IsActive => _encounterActive;
        public float MinimumHealth => _encounterActive && Body.Health != null
            ? Body.Health.MaxHealth * Mathf.Clamp01(_source.TransitionHealthRatio) : 0f;

        public PianoHumanEncounter(PianoBossHumanFormTransition source)
        {
            _source = source;
            _intro = new PianoHumanEncounterIntro(source, this);
            _change = new PianoHumanFormChange(source, this);
        }

        public void AfterInitialize()
        {
            UnsubscribeHealth();
            _intro.Cancel();
            _change.Cancel();

            _transitioning = false;
            _encounterActive = false;
            RewardCount = 0;
            RegisterPhaseOnePatterns();

            if (!_source.IsConfigured)
                return;

            Body.Restore();
        }

        private void RegisterPhaseOnePatterns()
        {
            if (Body.Owner == null || _source.PhaseOneClosingRingPattern == null)
                return;

            Body.Owner.GetCompo<MiddleBossAttackCompo>(true)?.AddPattern(_source.PhaseOneClosingRingPattern);
        }

        public void BeginEncounter(int rewardCount)
        {
            if (!_source.IsConfigured || Body.Owner == null)
                return;

            RewardCount = Mathf.Max(0, rewardCount);
            _encounterActive = true;

            UnsubscribeHealth();
            if (Body.Health != null)
                Body.Health.OnHpChanged += HandleHealthChanged;

            _intro.Begin();
        }

        public bool TryHandleHealthDepleted()
        {
            if (!_encounterActive || Body.Owner == null)
                return false;

            if (!_transitioning)
                BeginTransition(false);

            return _transitioning;
        }

        private void HandleHealthChanged(float currentHealth)
        {
            if (!_encounterActive || _transitioning || Body.Health == null || Body.Health.MaxHealth <= 0f)
                return;

            if (currentHealth > MinimumHealth + 0.001f)
                return;

            BeginTransition(false);
        }

        public void BeginTransition(bool skipPresentation)
        {
            if (!_encounterActive || _transitioning || Body.Owner == null)
                return;

            _transitioning = true;
            _intro.Cancel();
            _change.Begin(skipPresentation);
        }

        private void UnsubscribeHealth()
        {
            if (Body.Health != null)
                Body.Health.OnHpChanged -= HandleHealthChanged;
        }

        public void RestoreAfterFailedTransition()
        {
            _transitioning = false;
            Body.Restore();
        }

        public void Disable()
        {
            _encounterActive = false;
            Dispose();
        }

        public void Dispose()
        {
            UnsubscribeHealth();
            _intro.Cancel();
            _change.Cancel();
        }
    }
}
