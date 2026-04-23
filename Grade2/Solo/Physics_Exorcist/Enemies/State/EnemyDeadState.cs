using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Enemies.State
{
    public class EnemyDeadState : EnemyState
    {
        public EnemyDeadState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }
    }
}