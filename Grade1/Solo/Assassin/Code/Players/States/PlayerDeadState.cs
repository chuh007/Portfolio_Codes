using Code.Animators;
using Code.Entities;
using Code.Entities.FSM;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerDeadState : EntityState
    {
        private Player _player;
        private EntityMover _mover;
        public PlayerDeadState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately(true);
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