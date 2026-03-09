using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CUH.Code.GameEvents
{
    public struct TargetPosChangeEvent : IEvent
    {
        public Transform transform { get; private set; }
        public Vector3 direction { get; private set; }

        public TargetPosChangeEvent(Transform trm, Vector3 dir)
        {
            transform = trm;
            direction = dir;
        }
    }
}