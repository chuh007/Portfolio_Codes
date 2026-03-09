using _01Scripts.Combat;
using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace _01Scripts.Enemies.State
{
    public class EnemyMoveAttackState : EnemyState
    {
        private EnemyAttackCompo _attackCompo;
        private EntityAnimator _animator;
        private EntityAnimatorTrigger _animTrigger;
        
        private Vector3 orginPos;

        public EnemyMoveAttackState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<EnemyAttackCompo>();
            _animator = entity.GetCompo<EntityAnimator>();
            _animTrigger = entity.GetCompo<EntityAnimatorTrigger>();
        }

        public override void Enter()
        {
            base.Enter();
            orginPos = _entity.transform.position;
            _animTrigger.OnAnimationEndTrigger += HandleAnimationEndTrigger;
            _animTrigger.OnAttackTrigger += HandleAttackTrigger;
            Vector3 targetPos = _enemy.target.transform.position;
            int hashValue = _attackCompo.GetRandomAttack();
            var evt = EnemyEvents.EnemyActionEvent;
            evt.description = _attackCompo.currentAttackData.description;
            _enemy.enemyChannel.RaiseEvent(evt);
            _entity.transform.DOMove(targetPos - (targetPos - _entity.transform.position).normalized * 1.5f, 0.25f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.1f, () => _animator.SetParam(hashValue));
            });
        }

        private void HandleAttackTrigger()
        {
            _attackCompo.Attack();
        }
        
        private void HandleAnimationEndTrigger()
        {
            _enemy.ChangeState("IDLE");
            _entity.transform.DOMove(orginPos, 0.25f).OnComplete(() =>
            {
                TurnEndEvent evt = TurnEvents.TurnEndEvent;
                _entity.TurnChannel.RaiseEvent(evt);
            });
        }

        public override void Exit()
        {
            _animTrigger.OnAnimationEndTrigger -= HandleAnimationEndTrigger;
            _animTrigger.OnAttackTrigger -= HandleAttackTrigger;
            base.Exit();
        }
    }
}