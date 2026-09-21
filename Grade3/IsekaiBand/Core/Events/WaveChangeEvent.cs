using _Work.CHUH.Code.WaveSystem;
using Chuh007Lib.Bus;
using _Work.CHUH.Code.StageSystem;

namespace _Work.CHUH.Code.Core.Events
{
    public struct WaveChangeEvent : IEvent
    {
        public WaveDataSO WaveData;
        public StageDataSO StageData;
        public int WaveIndex;

        public WaveChangeEvent(WaveDataSO waveData)
            : this(null, waveData, -1)
        {
        }

        public WaveChangeEvent(StageDataSO stageData, WaveDataSO waveData, int waveIndex)
        {
            StageData = stageData;
            WaveData = waveData;
            WaveIndex = waveIndex;
        }
    }
}
