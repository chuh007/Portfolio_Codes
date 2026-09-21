using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    public class JapokEnemy : FSMEnemy
    {
        protected void Start()
        {
            _stateMachine.ChangeState(StateName.Move);
        }

        protected override void OnCollisionStay2D(Collision2D other)
        {
            
        }

        protected override void HandleHit()
        {
            
        }
    }
}