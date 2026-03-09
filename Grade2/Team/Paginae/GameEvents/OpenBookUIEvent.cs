using Work.CUH.Chuh007Lib.EventBus;
using Work.ISC.Code.SO;

namespace Work.CUH.Code.GameEvents
{
    public struct OpenBookUIEvent : IEvent
    {
        public string LoadSceneName;
        public readonly StageInfoSO StageInfo;
        
        public OpenBookUIEvent(string loadSceneName, StageInfoSO stageInfo)
        {
            LoadSceneName = loadSceneName;
            StageInfo = stageInfo;
        }
        
    }
    
    public struct CloseBookUIEvent : IEvent
    {
        
    }
}