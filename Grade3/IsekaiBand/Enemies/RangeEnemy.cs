using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    public class RangeEnemy : FSMEnemy
    {
        private static readonly Color RangedEnemyOutlineColor = new(1f, 0.08f, 0.08f, 1f);
        private const float RangedEnemyOutlineThickness = 1.25f;

        protected override void Awake()
        {
            base.Awake();
            GetComponentInChildren<Chuh007Lib.Entities.Entities.IRenderer>()?.SetOutline(
                RangedEnemyOutlineColor,
                RangedEnemyOutlineThickness);
        }

        protected void Start()
        {
            _stateMachine.ChangeState(StateName.Move);
        }
        
        protected override void HandleHit()
        {
            
        }
    }
}
