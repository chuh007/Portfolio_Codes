using Code.Animators;
using Code.Entities;
using Code.Entities.FSM;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerSlideState : EntityState
    {
        private Player _player;
        private EntityMover _mover;
        private readonly Vector2 _slideOffset = new Vector2(-0.005f, -0.61f);
        private readonly Vector2 _slideSize = new Vector2(0.55f, 0.88f);
        
        public PlayerSlideState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.CanManualMove = false;
            _mover.AddForceToEntity(new Vector2(5 * _renderer.FacingDirection, 0)); //나중에 변경
            _mover.SetColliderSize(_slideSize, _slideOffset);
        }

        public override void Update()
        {
            base.Update();
            if(_isTriggerCall)
                _player.ChangeState("IDLE");
        }

        public override void Exit()
        {
            _mover.CanManualMove = true;
            _mover.ResetColliderSize();
            base.Exit();
        }
    }
}