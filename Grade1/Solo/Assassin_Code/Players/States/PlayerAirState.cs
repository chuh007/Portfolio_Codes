using Code.Animators;
using Code.Entities;
using Code.Entities.FSM;
using UnityEngine;

namespace Code.Players.States
{
    public abstract class PlayerAirState : PlayerState
    {
        protected EntityMover _mover;
        protected PlayerAttackCompo _attackCompo;
            
        public PlayerAirState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _mover = entity.GetCompo<EntityMover>();
            _attackCompo = entity.GetCompo<PlayerAttackCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.SetMoveSpeedMultiplier(0.7f);
            _player.PlayerInput.OnJumpKeyPressed += HandleAirJump;
            _player.PlayerInput.OnAttackKeyPressed += HandleAirAttack;
        }

        public override void Update()
        {
            base.Update();
            float xInput = _player.PlayerInput.InputDirection.x;
            if(Mathf.Abs(xInput) > 0)
                _mover.SetMovementX(xInput);
            _player.UpdateCoyoteTime();
            bool isFrontMove = Mathf.Abs(xInput + _renderer.FacingDirection) > 1;
            if (isFrontMove && _mover.IsWallDetected(_renderer.FacingDirection))
            {
                _player.ChangeState("WALL_SLIDE");
            }
        }

        public override void Exit()
        {
            _player.PlayerInput.OnJumpKeyPressed -= HandleAirJump;
            _player.PlayerInput.OnAttackKeyPressed -= HandleAirAttack;
            _mover.SetMoveSpeedMultiplier(1f);
            base.Exit();
        }

        private void HandleAirAttack()
        {
            if(_attackCompo.CanJumpAttack())
                _player.ChangeState("JUMP_ATTACK");
        }

        private void HandleAirJump()
        {
            if(_player.CanJump || _player.CanCoyoteJump)
                _player.ChangeState("JUMP");
        }
    }
}