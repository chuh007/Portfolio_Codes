using _01Scripts.Entities;
using _01Scripts.FSM;

namespace _01Scripts.Players.States.UIInputStates
{
    public abstract class UIInputState : EntityState
    {
        protected Player _player;
        protected PlayerUIInputComponent PlayerUIInputComponent;

        
        public UIInputState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity as Player;
            PlayerUIInputComponent = entity.GetCompo<PlayerUIInputComponent>();
        }

        public override void Reset()
        {
            base.Reset();
            PlayerUIInputComponent = _player.GetCompo<PlayerUIInputComponent>();
        }

        public override void Enter()
        {
            base.Enter();
        }


        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}