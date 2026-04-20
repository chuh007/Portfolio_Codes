namespace Work.CUH.Chuh007Lib.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        //arguments
        public delegate void Event(T evt);

        public static Event OnEvent;
        public static void Raise(T evt) => OnEvent?.Invoke(evt);
    }
}