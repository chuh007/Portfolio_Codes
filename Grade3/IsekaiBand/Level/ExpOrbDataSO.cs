using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Code.LCH._02.Scripts.Level
{
    [CreateAssetMenu(fileName = "ExpOrbData", menuName = "SO/Level/ExpOrbData")]
    public class ExpOrbDataSO : ScriptableObject
    {
        [Header("획득 경험치")]
        public float expAmount = 10f;

        [Header("풀 아이템")]
        public PoolItemSO poolItem;
    }
}