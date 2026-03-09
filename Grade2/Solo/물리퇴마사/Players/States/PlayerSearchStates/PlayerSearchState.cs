using _01Scripts.Entities;
using _01Scripts.FSM;
using UnityEngine;

namespace _01Scripts.Players.States.PlayerSearchStates
{
    public abstract class PlayerSearchState : EntityState
    {
        protected Player _player;
        protected readonly float _inputThreshold = 0.1f;

        protected CharacterMovement _movement;
        protected PlayerCamRotator _camRotator;
        protected PlayerInteract _interact;
        
        public PlayerSearchState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity as Player;
            _movement = entity.GetCompo<CharacterMovement>();
            _camRotator = entity.GetCompo<PlayerCamRotator>();
            _interact = entity.GetCompo<PlayerInteract>();
        }

        public override void Enter()
        {
            base.Enter();
            _player.PlayerInput.OnRunPressed += HandleRunning;
        }

        private void HandleRunning(bool isRunning)
        {
            _movement.IsRunning = isRunning;
        }

        public override void Update()
        {
            base.Update();
            Vector2 rotateKey = _player.PlayerInput.LookKey;
            _camRotator.SetMouseDirection(rotateKey);
        }

        protected void HandleInteract()
        {
            _interact.Interact();   
        }
        
        public override void Exit()
        {
            _player.PlayerInput.OnRunPressed -= HandleRunning;
            base.Exit();
        }
    }

}
