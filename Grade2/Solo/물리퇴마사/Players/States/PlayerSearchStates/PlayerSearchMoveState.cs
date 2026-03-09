using _01Scripts.Entities;
using _01Scripts.Players.States.PlayerSearchStates;
using UnityEngine;

namespace _01Scripts.Players.States
{
    public class PlayerSearchMoveState : PlayerSearchState
    {
        private int _xMovementHash = Animator.StringToHash("XMovement");
        private int _yMovementHash = Animator.StringToHash("YMovement");
        
        public PlayerSearchMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }


        public override void Enter()
        {
            base.Enter();
            _player.PlayerInput.OnInteractPressed += HandleInteract;
        }

        public override void Update()
        {
            base.Update();
            Vector2 movementKey = _player.PlayerInput.MovementKey;
            _entityAnimator.SetParamDamping(_xMovementHash, movementKey.x, 0.1f, Time.deltaTime);
            _entityAnimator.SetParamDamping(_yMovementHash, movementKey.y, 0.1f, Time.deltaTime);
            _movement.SetMovementDirection(movementKey);
            if (movementKey.magnitude < _inputThreshold)
                _player.ChangeState("IDLE");
            if(_movement.IsRunning)
                _player.ChangeState("RUN");
        }

        public override void Exit()
        {
            _player.PlayerInput.OnInteractPressed -= HandleInteract;
            base.Exit();
        }
    }
}

