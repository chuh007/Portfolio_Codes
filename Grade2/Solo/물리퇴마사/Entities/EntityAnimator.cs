using UnityEngine;

namespace _01Scripts.Entities
{
    public class EntityAnimator : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Animator animator;

        private Entity _entity;
        public void Initialize(Entity entity)
        {
            _entity = entity;
            if (animator == null) animator = GetComponent<Animator>();
        }

        private bool IsAnimatorValid() => animator != null && !animator.Equals(null);
        
        public void SetParam(int hash, float value)
        {
            if (!IsAnimatorValid()) return;
            animator.SetFloat(hash, value);
        }

        public void SetParamDamping(int hash, float value, float dampTime, float deltaTime)
        {
            if (!IsAnimatorValid()) return;
            animator.SetFloat(hash, value, dampTime, deltaTime);
        }

        public void SetParam(int hash, int value)
        {
            if (!IsAnimatorValid()) return;
            animator.SetInteger(hash, value);
        }

        public void SetParam(int hash, bool value)
        {
            if (!IsAnimatorValid()) return;
            animator.SetBool(hash, value);
        }

        public void SetParam(int hash)
        {
            if (!IsAnimatorValid()) return;
            animator.SetTrigger(hash);
        }
    }
}

