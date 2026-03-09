using Code.Animators;
using Code.Entities;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerJumpState : PlayerAirState
    {
        public PlayerJumpState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _player.DecreaseJumpCount();
            _mover.StopImmediately(true);
            _mover.Jump();
            _mover.OnVelocity.AddListener(HandleVelocityChange);
        }

        public override void Exit()
        {
            _mover.OnVelocity.RemoveListener(HandleVelocityChange);
            base.Exit();
        }

        private void HandleVelocityChange(Vector2 velocity)
        {
            if(velocity.y < 0)
                _player.ChangeState("FALL");
        }
    }
}