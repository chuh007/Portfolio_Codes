using Ami.BroAudio;
using DG.Tweening;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;


namespace Work.CUH.Code.SwitchSystem
{
    public class Button : GridObjectBase, ICommandable, ISwitch
    {
        #region Grid
        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied()
        {
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
        }
        
        #endregion

        [SerializeField] private GameObject visual;
        [field: SerializeField] public ColorLinkObject linkObject { get; private set; }

        [Header("Target")]
        [SerializeField] private GameObject operateObject;
        [Header("Sound Setting")]
        [SerializeField] private SoundID btnSound;
        
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
        private GameObject _upObject;
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                if (_isActive)
                {
                    visual.transform.DOLocalMoveY(-0.1f, 0.1f);
                    activatable.Activate();
                }
                else
                {
                    visual.transform.DOLocalMoveY(-0.025f, 0.1f);
                    activatable.Deactivate();
                }
            }
        }
        
        private void Awake()
        {
            activatable = operateObject.GetComponent<IActivatable>();
            Bus<CommandCompleteEvent>.OnEvent += HandleCommandCompleted;
        }
        
        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;
            Debug.Assert(linkObject != null, $"linker can not be null");
            linkObject.SetLinkColor(activatable.linker.GetLinkColor());
        }

        private void OnDestroy()
        {
            Bus<CommandCompleteEvent>.OnEvent -= HandleCommandCompleted;
        }

        private void HandleCommandCompleted(CommandCompleteEvent evt)
        {
            var cell = GridSystem.Instance.GetCell(CurrentGridPosition);
            if (cell.Occupant && !IsActive)
            {
                Bus<CommandEvent>.Raise(new CommandEvent(new SwitchCommand(this)));
            }
            else if (!cell.Occupant && IsActive)
            {
                Bus<CommandEvent>.Raise(new CommandEvent(new SwitchCommand(this)));
            }
        }
        
        public void ToggleSwitch()
        {
            IsActive = !IsActive;
            BroAudio.Play(btnSound);
        }

        public void UndoSwitch()
        {
            IsActive = !IsActive;
        }
        
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