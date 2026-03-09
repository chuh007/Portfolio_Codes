using _01Scripts.Manager;

namespace _01Scripts.Core.EventSystem
{
    public static class CameraEvents
    {
        public static readonly CameraChangeEvent CameraChangeEvent = new CameraChangeEvent();
    }

    public class CameraChangeEvent : GameEvent
    {
        public ControlUIType CameraType;
    }
}