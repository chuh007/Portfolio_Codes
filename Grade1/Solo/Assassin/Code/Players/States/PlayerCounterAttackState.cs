using Code.Animators;
using Code.Combats;
using Code.Core.EventSystems;
using Code.Entities;
using Code.Entities.FSM;
using Code.SkillSystem;
using Code.SkillSystem.PlayerClone;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerCounterAttackState : EntityState
    {
        private Player _player;
        private PlayerAttackCompo _attackCompo;
        private EntityMover _mover;

        private float _counterTimer;
        private bool _counterSuccess;
        
        public PlayerCounterAttackState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _attackCompo = entity.GetCompo<PlayerAttackCompo>();
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately(false);
            _counterTimer = _attackCompo.counterAttackDuration;
            _renderer.SetParam(_attackCompo.successCounterParam, false);
            _counterSuccess = false;
        }

        public override void Update()
        {
            base.Update();
            _counterTimer -= Time.deltaTime;
            if (_counterSuccess == false)
                CheckCounter();
            
            if(_counterTimer < 0 || _isTriggerCall)
                _player.ChangeState("IDLE");
        }

        private void CheckCounter()
        {
            ICounterable countable = _attackCompo.GetCounterableTargetInRadius();

            if (countable is { CanCounter: true })
            {
                _counterSuccess = true;
                AttackDataSO attackData = _attackCompo.GetAttackData("PlayerCounterAttack");
                float damage = 10f; //하드코딩
                Vector2 attackDirection = new Vector2(_renderer.FacingDirection, 0);
                Vector2 knockBackForce = attackData.knockBackForce;
                knockBackForce.x *= _renderer.FacingDirection;
                
                countable.ApplyCounter(damage, attackDirection, knockBackForce, attackData.isPowerAttack, _player);
                _renderer.SetParam(_attackCompo.successCounterParam, true);
                
                //카운터 성공메시지 보낸다.
                CounterSuccessEvent counterEvt = PlayerEvents.CounterSuccessEvent;
                counterEvt.target = countable.TargetTrm;
                _player.PlayerChannel.RaiseEvent(counterEvt);
            }
        }
    }
}