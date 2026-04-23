using _01Scripts.Entities;
using _01Scripts.Players.States.PlayerSearchStates;
using UnityEngine;

namespace _01Scripts.Players.States
{
    public class PlayerSearchIdleState : PlayerSearchState
    {

        public PlayerSearchIdleState(Entity entity, int animationHash) : base(entity, animationHash)
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
            
            _movement.SetMovementDirection(movementKey);
            if (movementKey.magnitude > _inputThreshold)
                _player.ChangeState("MOVE");
        }
        
        public override void Exit()
        {
            _player.PlayerInput.OnInteractPressed -= HandleInteract;
            base.Exit();
        }
    }
}

