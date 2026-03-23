using Ami.BroAudio;
using UnityEngine;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.SwitchSystem
{
    public class Lever : BaseSwitch, ICommandable
    {
        private static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Material onMaterial;
        [SerializeField] private Material offMaterial;
        [SerializeField] private Animator animator;
        
        private bool _isActive;
        
        public override bool IsActive
        {
            get => _isActive;
            protected set
            {
                _isActive = value;
                if (_isActive)
                {
                    foreach (var render in renderers)
                    {
                        render.material = onMaterial;
                    }
                    animator.SetBool(Open, true);
                    activatable.Activate();
                }
                else
                {
                    foreach (var render in renderers)
                    {
                        render.material = offMaterial;
                    }
                    animator.SetBool(Open, false);
                    activatable.Deactivate();
                }
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            Debug.Assert(linkObject != null, $"linker can not be null");
            linkObject.SetLinkColor(activatable.linker.GetLinkColor());
        }
    }
}