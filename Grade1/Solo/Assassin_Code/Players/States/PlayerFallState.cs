using Code.Animators;
using Code.Entities;

namespace Code.Players.States
{
    public class PlayerFallState : PlayerAirState
    {
        public PlayerFallState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
        }

        public override void Update()
        {
            base.Update();
            if (_mover.IsGroundDetected())
            {
                _player.ResetJumpCount();
                _player.ChangeState("IDLE");
            }
        }

    }
}