using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.States.Range
{
    public class RangeEnemyAttackState : EnemyState
    {
        private EntityAnimatorTrigger _trigger;
        private EnemyAttackCompo _attackCompo;
        private bool _isWaitingForPattern;
        
        public RangeEnemyAttackState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _trigger = entity.GetCompo<EntityAnimatorTrigger>();
            _attackCompo = entity.GetCompo<EnemyAttackCompo>(true);
        }

        public override void Enter()
        {
            base.Enter();
            _isWaitingForPattern = false;
            _trigger.OnAnimationTrigger += HandleAttack;
            _trigger.OnAnimationEnd += HandleAnimEnd;
        }

        public override void Exit()
        {
            _trigger.OnAnimationTrigger -= HandleAttack;
            _trigger.OnAnimationEnd -= HandleAnimEnd;
            base.Exit();
        }

        private void HandleAnimEnd()
        {
            if (_isWaitingForPattern || _attackCompo.IsSinglePatternRunning)
                return;

            _enemy.ChangeState(StateName.Move);
        }

        private void HandleAttack()
        {
            if (_isWaitingForPattern || _attackCompo.IsSinglePatternRunning)
                return;

            if (_attackCompo.HasSinglePattern)
            {
                _isWaitingForPattern = true;
                _attackCompo.Attack(HandlePatternEnd);
                return;
            }

            _attackCompo.Attack();
        }

        private void HandlePatternEnd()
        {
            _isWaitingForPattern = false;
            _enemy.ChangeState(StateName.Move);
        }
    }
}
