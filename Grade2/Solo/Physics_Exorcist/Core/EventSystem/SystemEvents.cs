using UnityEngine;

namespace _01Scripts.Core.EventSystem
{
    public static class SystemEvents
    {
        public static SaveGameEvent SaveGameEvent = new SaveGameEvent();
        public static LoadGameEvent LoadGameEvent = new LoadGameEvent();
    }

    public class SaveGameEvent : GameEvent
    {
        public bool isSaveToFile;
        public int savePointNumber; //로드시 해당 위치에 가져다 두기 위해서.(지금은 안쓴다.)
    }

    public class LoadGameEvent : GameEvent
    {
        public bool isLoadFromFile;
    }
}