using Code.Animators;
using Code.Entities;
using Code.Entities.FSM;
using System;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerWallSlideState : EntityState
    {
        private Player _player;
        private EntityMover _mover;

        private const float WALL_SLIDE_GRAVITY_SCALE = 0.3f;
        private const float WALL_SLIDE_LIMIT_SPEED = 5f;
        private const float NORMAL_LIMIT_SPEED = 40f;
        public PlayerWallSlideState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately(true);
            _mover.SetLimitYSpeed(WALL_SLIDE_LIMIT_SPEED);
            _mover.SetGravityScale(WALL_SLIDE_GRAVITY_SCALE);
            _player.ResetCoyoteTime();
            _player.PlayerInput.OnJumpKeyPressed += HandleJumpKeyPress;
        }

        public override void Update()
        {
            base.Update();
            float xInput = _player.PlayerInput.InputDirection.x;
            if (Mathf.Abs(xInput + _renderer.FacingDirection) < 0.5f)
            {
                _player.ChangeState("FALL");
                return;
            }
            
            if (_mover.IsGroundDetected() || _mover.IsWallDetected(_renderer.FacingDirection) == false)
            {
                _player.ChangeState("IDLE");
                _player.ResetJumpCount();
            }
        }

        public override void Exit()
        {
            _mover.SetGravityScale(1f);
            _player.PlayerInput.OnJumpKeyPressed -= HandleJumpKeyPress;
            _mover.SetLimitYSpeed(NORMAL_LIMIT_SPEED);
            base.Exit();
        }

        private void HandleJumpKeyPress()
        {
            _mover.StopImmediately(true);
            _player.ChangeState("JUMP");
        }
    }
}