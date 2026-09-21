using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;

namespace _Work.CHUH.Code.Enemies.States
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            _enemy.ChangeState(StateName.Move);
        }

        public override void Update()
        {
            base.Update();
            
        }
    }
}
