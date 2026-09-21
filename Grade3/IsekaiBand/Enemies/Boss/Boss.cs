using System.Threading;
using _Code.LCH._02.Scripts.Bus;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.BT;
using _Work.CHUH.Code.Combat.Pattern;
using _Work.CHUH.Code.Resource;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using Unity.Behavior;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    public interface IBossDeathCompletionPresentation
    {
        void OnDeathStarted(CancellationToken cancellationToken);
        UniTask<bool> PlayBeforeCompletionAsync(CancellationToken cancellationToken);
    }

    public class Boss : Enemy
    {
        private const string EnemyStateVariableName = "EnemyState";
        private const string StateChannelVariableName = "StateChannel";
        private const int MiddleBossNoteRewardAmount = 1;

        [SerializeField] private bool treatAsMiddleBoss;
        [SerializeField] private BossDefeatPresentation finalDefeatPresentation = new BossDefeatPresentation();

        private BehaviorGraphAgent _btAgent;
        private BossPhaseController _phaseController;
        private bool _deathCompleted;
        private BossDeathCompletion _deathCompletion;
        private BossDeathCompletion DeathCompletion
            => _deathCompletion ??= new BossDeathCompletion(this, FinishDelayedDeath);
        internal BossDefeatPresentation FinalDefeatPresentation => finalDefeatPresentation;
        private float _patternRecoveryDelay;
        private bool _isPatternExecutionEnabled = true;

        protected override bool AlwaysKnockbackImmune => true;

        public override bool IsBoss => true;
        public bool TreatAsMiddleBoss => treatAsMiddleBoss;
        public bool IsDelayedDeathCompleted => _deathCompleted;
        public bool IsPhaseTransitioning => _phaseController != null && _phaseController.IsPhaseTransitioning;
        public bool IsPatternExecutionEnabled => _isPatternExecutionEnabled;
        public int CurrentPhaseIndex => _phaseController != null ? _phaseController.CurrentPhaseIndex : -1;
        public CancellationToken PhaseCancellationToken => _phaseController?.PhaseCancellationToken ?? destroyCancellationToken;

        protected override void Awake()
        {
            _btAgent = GetComponent<BehaviorGraphAgent>();
            Debug.Assert(_btAgent != null, $"{gameObject.name} does not have an BehaviorGraphAgent");
            base.Awake();
        }

        protected override void AfterInitialize()
        {
            base.AfterInitialize();
            _deathCompleted = false;
            DeathCompletion.Initialize();
            _isPatternExecutionEnabled = true;
            _phaseController = GetCompo<BossPhaseController>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void HandleHit()
        {
        }

        protected override void HandleDead()
        {
            if (IsDead)
                return;

            DeathCompletion.CompletionStarted = false;
            _deathCompleted = false;
            IsDead = true;
            gameObject.layer = DeadBodyLayer;
            StopDeathPhysics();
            _phaseController?.CancelCurrentPattern();
            DeathCompletion.OnDeathStarted();
            ChangeBtState(BTEnemyState.DEATH);
        }

        protected override void OnCollisionStay2D(Collision2D other)
        {
        }

        public override void TakeDamage(DamageData damage, Vector2 direction = default, Entity dealer = null)
        {
            if (IsPhaseTransitioning)
                return;

            base.TakeDamage(damage, direction, dealer);
        }

        public void ChangePhase(int phaseIndex)
        {
            _phaseController?.ChangePhase(phaseIndex);
        }

        public void ChangeNextPhase()
        {
            _phaseController?.ChangeNextPhase();
        }

        public bool EnterPhaseImmediately(int phaseIndex, bool startImmediatePattern = true)
        {
            return _phaseController != null
                   && _phaseController.EnterPhaseImmediately(phaseIndex, startImmediatePattern);
        }

        public BasePatternSO ConsumeForcedPattern()
        {
            return _phaseController?.ConsumeForcedPattern();
        }

        public void ForceNextPattern(BasePatternSO pattern)
        {
            _phaseController?.ForceNextPattern(pattern);
        }

        public void BeginPatternRecovery(float duration)
        {
            _patternRecoveryDelay = Mathf.Max(0f, duration);
        }

        public float ConsumePatternRecoveryDelay(float minimumDuration)
        {
            float delay = Mathf.Max(Mathf.Max(0f, minimumDuration), _patternRecoveryDelay);
            ClearPatternRecovery();
            return delay;
        }

        public void ClearPatternRecovery()
        {
            _patternRecoveryDelay = 0f;
        }

        public void SetPatternExecutionEnabled(bool enabled)
        {
            if (_isPatternExecutionEnabled == enabled)
                return;

            _isPatternExecutionEnabled = enabled;
            if (enabled)
                return;

            _phaseController?.CancelCurrentPattern();
            global::BossPatternSelection.Clear(this);
            ClearPatternRecovery();
        }

        public void CompleteDelayedDeath() => DeathCompletion.CompleteDelayedDeath();

        private void FinishDelayedDeath()
        {
            if (_deathCompleted)
                return;

            _deathCompleted = true;
            CompleteDeath();

            if (treatAsMiddleBoss)
            {
                Bus<MiddleBossNoteRewardEvent>.Raise(new MiddleBossNoteRewardEvent(
                    MiddleBossNoteRewardAmount,
                    transform.position));
            }

            ReturnToPool();
            if (treatAsMiddleBoss)
                return;

            SupplyManager.Instance?.TryGrantGameClearReward();
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(SoundKeys.GameClear, SoundType.SFX));
            Bus<GameClearEvent>.Raise(new GameClearEvent());
        }

        public void ChangeBtState(BTEnemyState state)
        {
            SetBlackboardVariable(EnemyStateVariableName, state);

            BlackboardVariable<global::StateChange> channel = GetBlackboardVariable<global::StateChange>(StateChannelVariableName);
            channel?.Value?.SendEventMessage(state);
        }

        public BlackboardVariable<T> GetBlackboardVariable<T>(string key)
        {
            if (_btAgent.GetVariable(key, out BlackboardVariable<T> result))
                return result;

            return default;
        }

        public bool SetBlackboardVariable<T>(string key, T value)
        {
            BlackboardVariable<T> variable = GetBlackboardVariable<T>(key);
            if (variable == null) return false;

            variable.Value = value;
            return true;
        }
    }
}
