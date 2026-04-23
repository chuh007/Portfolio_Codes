using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Enemies.State
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }
    }
}