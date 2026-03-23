using Ami.BroAudio;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.SwitchSystem
{
    public class EnterSwitch : BaseSwitch, ICommandable
    {
        [SerializeField] private GameObject onVisual;
        [SerializeField] private GameObject offVisual;
        [SerializeField] private ParticleSystem onSwitchParticle;
        [SerializeField] private ParticleSystem offSwitchParticle;
        
        private bool _isActive;
        
        public override bool IsActive
        {
            get => _isActive;
            protected set
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
        
        protected override void Awake()
        {
            base.Awake();
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
    }
}