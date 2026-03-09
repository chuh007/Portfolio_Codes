using Code.Combats.Onset;
using Code.Combats;
using UnityEngine;
using Code.Entities;
using Code.Enemies.BTCommons;
using Unity.Behavior;
using System;
using Code.Core.EventSystems;

namespace Code.Enemies.Fly
{
    public class EnemyFly : BTEnemy, IOnsetable
    {
        private StateChangeEvent _stateChannel;
        private BlackboardVariable<BTEnemyState> _state;
        private BlackboardVariable<float> _stunTime;

        private EntityMover _mover;
        private EntityFeedbackData _feedbackData;
        private EntityAnimationTrigger _animationTrigger;
        private EnemyAttackCompo _attackCompo;
        private EntityHealth _healthCompo;


        public bool IsFindPlayer => isPlayerFound;

        protected override void AfterInitialize()
        {
            base.AfterInitialize();
            _mover = GetCompo<EntityMover>();
            _feedbackData = GetCompo<EntityFeedbackData>();
            GetCompo<EntityHealth>().OnKnockback += HandleKnockBack;
            _animationTrigger = GetCompo<EntityAnimationTrigger>();
            _attackCompo = GetCompo<EnemyAttackCompo>();
            _healthCompo = GetCompo<EntityHealth>();
        }

        protected override void Start()
        {
            BlackboardVariable<StateChangeEvent> stateChannelVariable =
                GetBlackboardVariable<StateChangeEvent>("StateChannel");
            _stateChannel = stateChannelVariable.Value;
            Debug.Assert(_stateChannel != null, $"StateChannel variable is null {gameObject.name}");

            _state = GetBlackboardVariable<BTEnemyState>("EnemyState");
            _stunTime = GetBlackboardVariable<float>("StunTime");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GetCompo<EntityHealth>().OnKnockback -= HandleKnockBack;
        }

        private void HandleKnockBack(Vector2 knockBackForce)
        {
            float knockBackTime = 0.5f;
            _mover.KnockBack(knockBackForce, knockBackTime);
        }

        protected override void HandleHit()
        {
            if (IsDead) return;

            if (_state.Value == BTEnemyState.STUN || _state.Value == BTEnemyState.HIT) return;

            if (_feedbackData.IsLastHitPowerAttack)
            {
                _stateChannel.SendEventMessage(BTEnemyState.HIT);
            }
            else if (_state.Value == BTEnemyState.PATROL)
            {
                _stateChannel.SendEventMessage(BTEnemyState.CHASE);
            }
        }

        protected override void HandleDead()
        {
            if (IsDead) return;
            base.HandleDead();
            gameObject.layer = DeadBodyLayer;
            IsDead = true;
            _stateChannel.SendEventMessage(BTEnemyState.DEATH);

        }
    }
}

