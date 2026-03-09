using Code.Animators;
using Code.Combats;
using Code.Entities;
using Code.Entities.FSM;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerDashAttackState :EntityState
    {
        private Player _player;
        private EntityMover _mover;
        private PlayerAttackCompo _attackCompo;
        
        public PlayerDashAttackState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
            _attackCompo = entity.GetCompo<PlayerAttackCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.CanManualMove = false;

            SetAttackData();
        }

        private void SetAttackData()
        {
            AttackDataSO attackData = _attackCompo.GetAttackData("PlayerDashAttack");
            Vector2 movement = attackData.movement; //이따 만들께
            movement.x *= _renderer.FacingDirection;
            _mover.AddForceToEntity(movement);
            
            _attackCompo.SetAttackData(attackData);
        }

        public override void Update()
        {
            base.Update();
            if(_isTriggerCall)
                _player.ChangeState("IDLE");
        }

        public override void Exit()
        {
            _mover.CanManualMove = true;
            base.Exit();
        }
    }
}