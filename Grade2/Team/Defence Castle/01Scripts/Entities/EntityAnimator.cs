using ChipmunkKingdoms.Scripts.Utility;
using UnityEngine;

namespace Work.CHUH._01Scripts.Entities
{
    public partial class EntityAnimator : MonoBehaviour, IContainerComponent
    {
        [SerializeField] private Animator animator;
        
        public void SetParam(int hash, float value) => animator.SetFloat(hash, value);
        public void SetParam(int hash, int value) => animator.SetInteger(hash, value);
        public void SetParam(int hash, bool value) => animator.SetBool(hash, value);
        public void SetParam(int hash) => animator.SetTrigger(hash);
        public ComponentContainer ComponentContainer { get; set; }
        public void OnInitialize(ComponentContainer componentContainer)
        {
        }
    }
}