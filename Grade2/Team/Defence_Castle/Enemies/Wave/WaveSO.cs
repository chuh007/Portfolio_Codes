using System;
using System.Collections.Generic;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace Work.CHUH._01Scripts.Enemies.Wave
{
    [Serializable]
    public struct WaveEnemyData
    {
        public PoolItemSO enemyPoolSO;
        public int baseSpawnCnt;
        public float delayToNextEnemy;
    }
    
    [CreateAssetMenu(fileName = "Wave", menuName = "SO/Wave/Wave", order = 0)]
    public class WaveSO : ScriptableObject
    {
        public List<WaveEnemyData> enemies;
    }
}