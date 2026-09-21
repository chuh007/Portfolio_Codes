using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;

namespace _Work.CHUH.Code.Enemies.States.Japok
{
    public class JapokEnemyAttackState : EnemyState
    {
        private EntityAnimatorTrigger _trigger;
        private EntityMover _mover;
        private JapokEnemyAttackCompo _attackCompo;
        private bool _isWaitingForPattern;
        
        public JapokEnemyAttackState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _trigger = entity.GetCompo<EntityAnimatorTrigger>();
            _mover = entity.GetCompo<EntityMover>();
            _attackCompo = entity.GetCompo<JapokEnemyAttackCompo>(true);
        }

        public override void Enter()
        {
            base.Enter();
            _isWaitingForPattern = false;
            _mover.StopImmediately();
            _trigger.OnAnimationVfxTrigger += _attackCompo.PlayExplosionVfx;
            _trigger.OnAnimationTrigger += HandleAttack;
            _trigger.OnAnimationEnd += HandleAnimEnd;
        }

        public override void Exit()
        {
            _trigger.OnAnimationVfxTrigger -= _attackCompo.PlayExplosionVfx;
            _trigger.OnAnimationTrigger -= HandleAttack;
            _trigger.OnAnimationEnd -= HandleAnimEnd;
            base.Exit();
        }

        private void HandleAnimEnd()
        {
            if (_isWaitingForPattern || _attackCompo.IsSinglePatternRunning)
                return;

            _enemy.OnDead?.Invoke();
        }

        private void HandleAttack()
        {
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
            _enemy.OnDead?.Invoke();
        }
        
    }
}
