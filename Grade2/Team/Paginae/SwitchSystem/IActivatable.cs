using UnityEngine;

namespace Work.CUH.Code.SwitchSystem
{
    public interface IActivatable
    {
        public GameObject gameObject { get; }
        public ColorLinkObject linker { get; }
        
        public void Activate();
        public void Deactivate();
    }
}