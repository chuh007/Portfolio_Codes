using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CUH.Code.GameEvents
{
    public struct PlayerPosChangeEvent : IEvent
    {
        public Vector3 position { get; private set; }
        public Vector3 direction { get; private set; }

        public PlayerPosChangeEvent(Vector3 pos, Vector3 dir)
        {
            position = pos;
            direction = dir;
        }
    }
}