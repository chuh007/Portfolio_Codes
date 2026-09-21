using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.States.MiddleBoss
{
    public class MiddleBossMoveState : EnemyMoveState
    {
        private EnemyAttackCompo _attackCompo;

        private Rigidbody2D _rb;
        private float _delay = 0.1f;
        
        public MiddleBossMoveState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _attackCompo = entity.GetCompo<EnemyAttackCompo>(true);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
            _delay -= Time.deltaTime;
            if (_delay > 0) return;
            if (_attackCompo.CanAttack())
            {
                _enemy.ChangeState(StateName.Attack);
            }
        }
    }
}
