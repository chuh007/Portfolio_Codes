using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    /// <summary>
    /// 씬간 StageData 통신에 사용하는 SO.
    /// StageSelector, WaveController 외 접근 금지.
    /// </summary>
    [CreateAssetMenu(fileName = "StageDataSender", menuName = "SO/Stage/StageDataSender", order = 0)]
    public class StageDataSenderSO : ScriptableObject
    {
        public StageDataSO Data;
    }
}