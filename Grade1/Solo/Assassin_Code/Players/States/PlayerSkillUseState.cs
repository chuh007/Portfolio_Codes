using Code.Animators;
using Code.Entities;
using Code.Entities.FSM;

namespace Code.Players.States
{
    public class PlayerSkillUseState : EntityState
    {
        private Player _player;
        private EntityMover _mover;
        
        public PlayerSkillUseState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately(false);
        }

        public override void Update()
        {
            base.Update();
            if(_isTriggerCall)
                _player.ChangeState("IDLE");
        }
    }
}