using System;
using Ami.BroAudio;
using UnityEngine;
using Work.CIW.Code.Grid;

namespace Work.CUH.Code.SwitchSystem
{
    public abstract class BaseSwitch : GridObjectBase, ISwitch
    {
        [field: SerializeField] public ColorLinkObject linkObject { get; private set; }
        
        [Header("Target")]
        [SerializeField] protected GameObject operateObject;
        [SerializeField] private SoundID btnSound;
        
        public IActivatable activatable { get; private set; }
        
        public abstract bool IsActive { get; protected set; }
        
        protected virtual void Awake()
        {
            activatable = operateObject.GetComponent<IActivatable>();
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
        
        #region Grid(사용되지 않음)

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied() { }

        public override void OnCellOccupied(Vector3Int newPos) { }

        #endregion
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if(operateObject == null) return;
            if (!operateObject.TryGetComponent(out IActivatable activate))
            {
                operateObject = null;
            }
        }
#endif
    }
}