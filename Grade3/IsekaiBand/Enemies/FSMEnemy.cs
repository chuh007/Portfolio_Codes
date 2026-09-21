using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    public class FSMEnemy : Enemy
    {
        [SerializeField] protected EntityFSMSO entityFSM;
        
        protected StateMachine _stateMachine;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new StateMachine(entityFSM, this);
        }
        
        private void OnEnable()
        {
            Animator animator = GetCompo<Chuh007Lib.Entities.Entities.EntityRenderer>()?.Anim;
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.Rebind();

            _stateMachine.ChangeState(StateName.Move);

            if (animator != null
                && animator.runtimeAnimatorController != null
                && animator.isActiveAndEnabled
                && animator.gameObject.activeInHierarchy)
            {
                animator.Update(0f);
            }
        }

        public override void ResetItem()
        {
            base.ResetItem();
            _stateMachine?.ChangeState(StateName.Move);
        }

        protected override void Update()
        {
            base.Update();
            _stateMachine.CurrentState.Update();
        }

        public override void ChangeState(StateName newState)
        {
            base.ChangeState(newState);
            if (IsDead && newState != StateName.Dead) return;
            _stateMachine.ChangeState(newState);

        }

        protected override void HandleHit()
        {
            
        }
    }
}
