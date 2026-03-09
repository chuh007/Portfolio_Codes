using Ami.BroAudio;
using UnityEngine;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.SwitchSystem
{
    public class Lever : MonoBehaviour, ISwitch, ICommandable
    {
        private static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Material onMaterial;
        [SerializeField] private Material offMaterial;
        [SerializeField] private Animator animator;
        [field: SerializeField] public ColorLinkObject linkObject { get; private set; }
        
        [Header("Target")]
        [SerializeField] private GameObject operateObject;
        
        [Header("Sound Setting")]
        [SerializeField] private SoundID leverSound;
        
        public IActivatable activatable { get; private set; }
        
        public GameObject activeObject
        {
            get => operateObject;
            private set
            {
                if (operateObject.TryGetComponent(out IActivatable activate))
                {
                    operateObject = value;
                    activatable = activate;
                }
            }
        }
        
        private bool _isActive;
        
        public bool IsActive
        {
            get => _isActive;
            private set
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
        
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            activatable = operateObject.GetComponent<IActivatable>();
        }

        private void Start()
        {
            Debug.Assert(linkObject != null, $"linker can not be null");
            linkObject.SetLinkColor(activatable.linker.GetLinkColor());
        }

        public void ToggleSwitch()
        {
            IsActive = !IsActive;
            BroAudio.Play(leverSound);
        }

        public void UndoSwitch()
        {
            IsActive = !IsActive;
        }
    }
}