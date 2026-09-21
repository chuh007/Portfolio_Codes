using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.States.MiddleBoss
{
    public class MiddleBossAttackState : EnemyState
    {
        private EnemyAttackCompo _attackCompo;
        private EntityMover _mover;
        
        public MiddleBossAttackState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _attackCompo = entity.GetCompo<EnemyAttackCompo>(true);
            _mover = entity.GetCompo<EntityMover>();
        }
        
        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately();
            _attackCompo.Attack(AttackEnd);
        }

        public override void Exit()
        {
            base.Exit();
        }
        
        private void AttackEnd()
        {
            _enemy.ChangeState(StateName.Move);
        }
    }
}
