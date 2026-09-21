using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.Core.Events
{
    public struct MapSizeSetEvent : IEvent
    {
        public float Width;
        public float Height;
        public bool IsInfinite;

        public MapSizeSetEvent(float width, float height, bool isInfinite = false)
        {
            Width = width;
            Height = height;
            IsInfinite = isInfinite;
        }
    }
}
