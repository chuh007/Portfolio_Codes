using _01Scripts.Entities;
using _01Scripts.FSM;
using UnityEngine;

namespace _01Scripts.Enemies.State
{
    public abstract class EnemyState : EntityState
    {
        protected Enemy _enemy;
        public EnemyState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _enemy = entity as Enemy;
        }
        
        
    }
}