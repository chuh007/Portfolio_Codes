using Chuh007Lib.Entities.Animator;
using Chuh007Lib.Entities.Entities;

namespace _Work.CHUH.Code.Enemies.States
{
    public class EnemyState : EntityState
    {
        protected Enemy _enemy;
        
        public EnemyState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _enemy = entity as Enemy;
        }
    }
}