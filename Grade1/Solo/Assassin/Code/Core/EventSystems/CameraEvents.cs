using Unity.Cinemachine;
using UnityEngine;

namespace Code.Core.EventSystems
{
    public enum PanDirection
    {
        Up, Down, Left, Right
    }
    
    public static class CameraEvents
    {
        public static PanEvent PanEvent = new PanEvent();
        public static SwapCameraEvent SwapCameraEvent = new SwapCameraEvent();
    }

    public class PanEvent : GameEvent
    {
        public float distance;
        public float panTime;
        public PanDirection direction; 
        public bool isRewindToStart;
    }

    public class SwapCameraEvent : GameEvent
    {
        public CinemachineCamera leftCamera;
        public CinemachineCamera rightCamera;
        public Vector2 moveDirection;
    }
}