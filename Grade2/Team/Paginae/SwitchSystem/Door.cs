using Ami.BroAudio;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.SwitchSystem
{
    public class Door : GridObjectBase, IActivatable
    {
        [SerializeField] private GameObject onVisual;
        [SerializeField] private GameObject offVisual;
        [SerializeField, ColorUsage(true, true)] private Color linkColor;
        
        private Collider _collider;
        
        [field: SerializeField] public ColorLinkObject linker { get; private set; }
        
        [Header("Sound Setting")]
        [SerializeField] private SoundID doorSound;
        
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            Debug.Assert(linker != null, $"linker can not be null");
            linker.SetLinkColor(linkColor);
        }

        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;
            GridSystem.Instance.SetObjectInitialPosition(this, CurrentGridPosition);
        }


        public void Activate()
        {
            _collider.enabled = false;
            onVisual.SetActive(true);
            offVisual.SetActive(false);
            Bus<TextEvent>.Raise(new TextEvent("장치가 작동해 문이 열립니다."));
            BroAudio.Play(doorSound);
            GridSystem.Instance.RemoveObjectPosition(this, CurrentGridPosition);
        }

        public void Deactivate()
        {
            _collider.enabled = true;
            onVisual.SetActive(false);
            offVisual.SetActive(true);
            GridSystem.Instance.SetObjectInitialPosition(this, CurrentGridPosition);
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
    }
}