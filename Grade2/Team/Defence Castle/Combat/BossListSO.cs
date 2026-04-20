using System.Collections.Generic;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace Work.CHUH._01Scripts.Combat
{
    [CreateAssetMenu(fileName = "BossList", menuName = "SO/BossList", order = 0)]
    public class BossListSO : ScriptableObject
    {
        public List<PoolItemSO> bossPoolSOList;
    }
}