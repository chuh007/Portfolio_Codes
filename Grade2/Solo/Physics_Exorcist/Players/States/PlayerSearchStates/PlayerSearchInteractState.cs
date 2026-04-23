using _01Scripts.Entities;
using _01Scripts.Players.States.PlayerSearchStates;
using UnityEngine;

namespace _01Scripts.Players.States
{
    public class PlayerSearchInteractState : PlayerSearchState
    {
        private EntityAnimatorTrigger _animatorTrigger;
        
        public PlayerSearchInteractState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _animatorTrigger = entity.GetCompo<EntityAnimatorTrigger>();
        }

        public override void Enter()
        {
            base.Enter();
            _movement.StopImmediately();
            _animatorTrigger.OnAnimationEndTrigger += HandleInteractEnd;
        }

        private void HandleInteractEnd()
        {
            _animatorTrigger.OnAnimationEndTrigger -= HandleInteractEnd;
            _player.ChangeState("MOVE");
        }

        public override void Exit()
        {
            _animatorTrigger.OnAnimationEndTrigger -= HandleInteractEnd;
            base.Exit();
        }
    }
}