using Ami.BroAudio;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.SwitchSystem
{
    public class Switch : GridObjectBase, ICommandable, ISwitch
    {
        [SerializeField] private GameObject onVisual;
        [SerializeField] private GameObject offVisual;
        [field: SerializeField] public ColorLinkObject linkObject { get; private set; }
        [SerializeField] private ParticleSystem onSwitchParticle;
        [SerializeField] private ParticleSystem offSwitchParticle;
        
        [Header("Target")]
        [SerializeField] private GameObject operateObject;
        
        [Header("Sound Setting")]
        [SerializeField] private SoundID switchSound;
        
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
                    SetVisual(true);
                    activatable.Activate();
                }
                else
                {
                    SetVisual(false);
                    activatable.Deactivate();
                }
            }
        }

        private void SetVisual(bool isActive)
        {
            onVisual.SetActive(isActive);
            offVisual.SetActive(!isActive);
            onSwitchParticle.gameObject.SetActive(isActive);
            offSwitchParticle.gameObject.SetActive(!isActive);
        }
        
        private void Awake()
        {
            activatable = operateObject.GetComponent<IActivatable>();
            Bus<PlayerPosChangeEvent>.OnEvent += HandlePlayerPosChange;
        }

        private void Start()
        {
            Debug.Assert(linkObject != null, $"linker can not be null");
            linkObject.SetLinkColor(activatable.linker.GetLinkColor());
        }

        private void OnDestroy()
        {
            Bus<PlayerPosChangeEvent>.OnEvent -= HandlePlayerPosChange;
        }
        
        private void HandlePlayerPosChange(PlayerPosChangeEvent evt)
        {
            if (Vector3.Distance(evt.position + evt.direction, transform.position) <= 0.05f)
            {
                Bus<CommandEvent>.Raise(new CommandEvent(new SwitchCommand(this)));
            }
        }
        
        [ContextMenu("Activate")]
        public void ToggleSwitch()
        {
            IsActive = !IsActive;
            BroAudio.Play(switchSound);
        }

        public void UndoSwitch()
        {
            IsActive = !IsActive;
        }
        
        #region Grid
        
        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied()
        {
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
        }
        
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(operateObject == null) return;
            if (operateObject.TryGetComponent(out IActivatable activate))
            {
                activeObject = operateObject;
            }
            else
            {
                operateObject = null;
            }
        }
#endif
    }
}