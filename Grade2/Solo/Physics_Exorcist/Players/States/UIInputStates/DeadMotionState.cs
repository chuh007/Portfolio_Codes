using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using _01Scripts.TurnSystem;
using UnityEngine;

namespace _01Scripts.Players.States.UIInputStates
{
    public class DeadMotionState : UIInputState
    {
        private Player _player;
        private EntityAnimatorTrigger _animTrigger;
        private GameEventChannelSO _uiChannel;
        
        public DeadMotionState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity as Player;
            _animTrigger = entity.GetCompo<EntityAnimatorTrigger>();
            _uiChannel = _player.UIChannel;
        }

        public override void Reset()
        {
            base.Reset();
            _animTrigger = _player.GetCompo<EntityAnimatorTrigger>();
            _uiChannel = _player.UIChannel;
        }

        public override void Enter()
        {
            base.Enter();
            _animTrigger.OnAnimationEndTrigger += HandleAnimEnd;
        }

        private void HandleAnimEnd()
        {
            var evt = UIEvents.DeadUIEvent;
            _uiChannel.RaiseEvent(evt);
        }

        public override void Exit()
        {
            _animTrigger.OnAnimationEndTrigger -= HandleAnimEnd;
            base.Exit();
        }
    }
}