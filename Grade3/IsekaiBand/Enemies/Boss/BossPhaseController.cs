using System.Collections.Generic;
using System.Threading;
using _Work.CHUH.Code.BT;
using _Work.CHUH.Code.Combat;
using _Work.CHUH.Code.Combat.Pattern;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

namespace _Work.CHUH.Code.Enemies.Boss
{
    public class BossPhaseController : MonoBehaviour, IEntityComponent, IAfterInitalize
    {
        private const int SecondPhaseIndex = 1;

        [SerializeField] private List<BossPhaseData> phases = new();
        [SerializeField, Min(1)] private int healthLineCount = 1;
        [SerializeField] private bool applyFirstPhaseOnStart = true;
        [SerializeField] private bool evaluateSkippedPhaseAfterTransition = true;
        [SerializeField] private BossPhaseTransitionPresentation phaseTransitionPresentation = new();
        [SerializeField] private BossPhaseIndexEvent onPhaseChanged = new();

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool enableDebugSecondPhaseKey = true;
#endif

        private Boss _boss;
        private IHealth _phaseHealth;
        private readonly BossPhaseState _state = new();
        private BossPhaseTransition _transition;

        public bool IsPhaseTransitioning => _state.IsTransitioning;
        public int CurrentPhaseIndex => _state.PhaseIndex;
        public CancellationToken PhaseCancellationToken => _state.CancellationToken;
        internal Boss Boss => _boss;
        internal IHealth Health => _phaseHealth;
        internal IReadOnlyList<BossPhaseData> Phases => phases;
        internal int HealthLineCount => healthLineCount;
        internal bool EvaluateSkippedPhase => evaluateSkippedPhaseAfterTransition;
        internal BossPhaseTransitionPresentation TransitionPresentation => phaseTransitionPresentation;

        public void Initialize(Entity entity)
        {
            _boss = entity as Boss;
            _transition ??= new BossPhaseTransition(this, _state);
            Debug.Assert(_boss != null, $"{gameObject.name} BossPhaseController needs Boss entity.");
        }

        public void AfterInitialize()
        {
            _phaseHealth = GetComponentInChildren<IHealth>();
            Debug.Assert(_phaseHealth != null, $"{gameObject.name} does not have an IHealth");
            if (_phaseHealth != null)
                _phaseHealth.OnHpChanged += _transition.HandlePhaseHealthChanged;

            CancelCurrentPattern();
            global::BossPatternSelection.Clear(_boss);
            _state.IsTransitioning = false;

            _state.PhaseIndex = applyFirstPhaseOnStart && phases.Count > 0 ? 0 : -1;
            if (_state.PhaseIndex >= 0)
            {
                _state.ApplyPhasePatterns(_boss, phases, _state.PhaseIndex);
            }
        }

        private void OnDestroy()
        {
            if (_phaseHealth != null)
                _phaseHealth.OnHpChanged -= _transition.HandlePhaseHealthChanged;

            global::BossPatternSelection.Clear(_boss);
            _state.Dispose();
        }

        public void ChangePhase(int phaseIndex)
        {
            _transition.TransitionToPhaseAsync(phaseIndex).Forget();
        }

        public void ChangeNextPhase()
        {
            ChangePhase(_state.PhaseIndex + 1);
        }

        public bool EnterPhaseImmediately(int phaseIndex, bool startImmediatePattern = true)
            => _state.EnterPhaseImmediately(this, phaseIndex, startImmediatePattern);

#if UNITY_EDITOR
        private void Update()
        {
            if (!enableDebugSecondPhaseKey || Keyboard.current == null)
                return;

            if (!Keyboard.current.lKey.wasPressedThisFrame)
                return;

            if (_boss == null || _boss.IsDead)
                return;

            ChangePhase(SecondPhaseIndex);
        }
#endif
        public BasePatternSO ConsumeForcedPattern() => _state.ConsumeForcedPattern();

        public void ForceNextPattern(BasePatternSO pattern) => _state.ForceNextPattern(_boss, pattern);

        public void CancelCurrentPattern() => _state.CancelCurrentPattern(_boss);

        internal void RaisePhaseChanged(int phaseIndex) => onPhaseChanged?.Invoke(phaseIndex);
    }
}
