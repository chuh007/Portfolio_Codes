using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.States
{
    public class EnemyMoveState : EnemyState
    {
        protected EntityMover _mover;
        
        public EnemyMoveState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
        }
        
        public override void Update()
        {
            base.Update();
            Vector2 moveDir = _enemy.target.transform.position - _enemy.transform.position;
            _mover.SetMovement(moveDir.normalized);
        }
    }
}