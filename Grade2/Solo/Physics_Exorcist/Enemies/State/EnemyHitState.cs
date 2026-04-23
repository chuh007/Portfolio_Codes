using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Enemies.State
{
    public class EnemyHitState : EnemyState
    {
        
        public EnemyHitState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _animatorTrigger.OnAnimationEndTrigger += HandleAnimEnd;
        }

        private void HandleAnimEnd()
        {
            _enemy.ChangeState("IDLE");
        }

        public override void Exit()
        {
            _animatorTrigger.OnAnimationEndTrigger -= HandleAnimEnd;
            base.Exit();
        }
    }
}