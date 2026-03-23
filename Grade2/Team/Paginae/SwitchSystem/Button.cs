using System;
using Ami.BroAudio;
using DG.Tweening;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;


namespace Work.CUH.Code.SwitchSystem
{
    public class Button : BaseSwitch, ICommandable, ISwitch
    {
        [SerializeField] private GameObject visual;
        
        private bool _isActive;
        private GameObject _upObject;
        
        public override bool IsActive
        {
            get => _isActive;
            protected set
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
        
        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;
            Debug.Assert(linkObject != null, $"linker can not be null");
            linkObject.SetLinkColor(activatable.linker.GetLinkColor());
        }
        
        private void Update()
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
    }
}