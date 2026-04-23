using _01Scripts.Enemies;
using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.TurnSystem
{
    public class EnemyTurnComponent : EntityTurnComponent
    {
        private Enemy _enemy;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            _enemy = entity as Enemy;
            Name = "적";
        }

        public override void TurnAction()
        {
            if(_enemy.IsDead) return;
            base.TurnAction();
            _enemy.ChangeState("MOVEATTACK");
        }
    }
}