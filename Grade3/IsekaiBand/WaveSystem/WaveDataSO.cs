using System;
using System.Collections.Generic;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Work.CHUH.Code.WaveSystem
{
    [Serializable]
    public struct WaveEnemyData
    {
        [Min(0f)] public float wight; // 소환 가중치, 이게 높으면 더 잘 나옴
        public float multiplierValue;
        public PoolItemSO enemyPrefab; // 적 풀타입
    }
    
    /// <summary>
    /// Stage내 1분마다 새로 설정되는 웨이브 데이터 SO
    /// </summary>
    [CreateAssetMenu(fileName = "WaveData", menuName = "SO/Wave/WaveData", order = 0)]
    public class WaveDataSO : ScriptableObject
    {
        public bool IsSpecialWave;

        /// <summary>
        /// 소환 가능한 적의 종류들
        /// </summary>
        public List<WaveEnemyData> EnemyPool;
        public int ToNextWave = 60; // 다음 웨이브까지 걸리는 시간
        public double SpawnInterval = 0.5f; // 소환 대기시간
        public int MinEnemiesCount = 5; // 있어야 하는 적의 하한선
        public int MaxEnemiesCount = 100; // 있어야 하는 적의 상한선
    }
}
