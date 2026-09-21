using _Work.CHUH.Code.Enemies.AttackCompo;
using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.States.Range
{
    /// <summary>
    /// 
    /// </summary>
    public class RangeEnemyMoveState : EnemyMoveState
    {
        private const float IdleSpeedThreshold = 0.1f;
        private static readonly int IdleAnimParamHash = Animator.StringToHash("IDLE");

        private EnemyAttackCompo _attackCompo;
        private float _delay = 0.1f;
        private bool _isIdleAnimation;
        
        public RangeEnemyMoveState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _attackCompo = entity.GetCompo<EnemyAttackCompo>(true);
        }

        public override void Exit()
        {
            SetIdleAnimation(false);
            base.Exit();
        }
        
        public override void Update()
        {
            if (_attackCompo.IsTargetInAttackRange())
                _mover.SetMovement(Vector2.zero);
            else
                base.Update();

            UpdateMovementAnimation();

            _delay -= Time.deltaTime;
            if (_delay > 0) return;
            if (_attackCompo.CanAttack())
            {
                _enemy.ChangeState(StateName.Attack);
            }
        }

        private void UpdateMovementAnimation()
        {
            bool shouldIdle = _mover.Velocity.sqrMagnitude
                              <= IdleSpeedThreshold * IdleSpeedThreshold;
            SetIdleAnimation(shouldIdle);
        }

        private void SetIdleAnimation(bool shouldIdle)
        {
            if (_isIdleAnimation == shouldIdle)
                return;

            _isIdleAnimation = shouldIdle;
            _renderer.SetParam(_animParam, !shouldIdle);
            _renderer.Anim.SetBool(IdleAnimParamHash, shouldIdle);
        }
    }
}
