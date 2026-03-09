using Code.Animators;
using Code.Combats;
using Code.Entities;
using Code.Entities.FSM;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerJumpAttackState : EntityState
    {
        private Player _player;
        private EntityMover _mover;
        private PlayerAttackCompo _attackCompo;
        
        public PlayerJumpAttackState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
            _attackCompo = entity.GetCompo<PlayerAttackCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately(true);
            _mover.SetGravityScale(0.1f); //순간적으로 공중에 멈추도록
            _mover.CanManualMove = false;

            SetAttackData();
        }

        private void SetAttackData()
        {
            AttackDataSO attackData = _attackCompo.GetAttackData("PlayerJumpAttack");
            Vector2 movement = attackData.movement;
            movement.x *= _renderer.FacingDirection;
            _mover.AddForceToEntity(movement);
            
            _attackCompo.SetAttackData(attackData);
        }

        public override void Update()
        {
            base.Update();
            if(_isTriggerCall)
                _player.ChangeState("FALL");
        }

        public override void Exit()
        {
            _mover.CanManualMove = true;
            _mover.SetGravityScale(1f);
            base.Exit();
        }
    }
}