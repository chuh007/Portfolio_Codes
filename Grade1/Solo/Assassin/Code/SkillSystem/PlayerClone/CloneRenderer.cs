using UnityEngine;
using UnityEngine.Events;

namespace Code.SkillSystem.PlayerClone
{
    public class CloneRenderer : MonoBehaviour
    {
        public UnityEvent OnAttackTrigger;
        public UnityEvent OnAnimationEndTrigger;
        
        private void AttackTrigger() => OnAttackTrigger?.Invoke();
        private void AnimationEnd() => OnAnimationEndTrigger?.Invoke();
    }
}