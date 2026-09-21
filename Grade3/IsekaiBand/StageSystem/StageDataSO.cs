using System;
using System.Collections.Generic;
using _Work.CHUH.Code.WaveSystem;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.StageSystem
{
    [Serializable]
    public struct BossSpawnData
    {
        public int spawnTime;
        public bool showApproachingWarning;
        public bool useDamageAttenuation;
        public PoolItemSO bossPoolItem;
    }
    
    /// <summary>
    /// 맵의 정보가 담기는 SO.
    /// 무한맵 청크 크기, 배경 에셋, 적 소환 풀, 보스 등의 정보를 넣는다.
    /// </summary>
    [CreateAssetMenu(fileName = "StageData", menuName = "SO/Stage/StageData", order = 0)]
    public class StageDataSO : ScriptableObject
    {
        public string StageName;
        public Vector2 StageSize = new Vector2(20, 20);
        public Sprite BackGroundSprite;
        public InfiniteMapChunkLibrarySO ChunkLibrary;
        public StageDecorationProfileSO DecorationProfile;
        public List<WaveDataSO> WaveList; // 1분마다 바꿀 웨이브 목록
        public List<BossSpawnData> BossSpawnList;

        [Header("Enemy Health Progression")]
        [Min(0f)] public float EnemyHealthGrowthStartTime = 240f;
        [Min(1f)] public float FinalWaveEnemyHealthMultiplier = 1f;

        public float GetEnemyHealthGrowthMultiplier(int waveIndex)
        {
            float finalMultiplier = Mathf.Max(1f, FinalWaveEnemyHealthMultiplier);
            if (WaveList == null || WaveList.Count == 0 || Mathf.Approximately(finalMultiplier, 1f))
                return 1f;

            int lastWaveIndex = WaveList.Count - 1;
            int currentWaveIndex = Mathf.Clamp(waveIndex, 0, lastWaveIndex);
            float currentWaveStartTime = GetWaveStartTime(currentWaveIndex);
            float lastWaveStartTime = GetWaveStartTime(lastWaveIndex);
            float growthStartTime = Mathf.Max(0f, EnemyHealthGrowthStartTime);

            if (currentWaveStartTime <= growthStartTime)
                return 1f;

            if (lastWaveStartTime <= growthStartTime)
                return currentWaveIndex == lastWaveIndex ? finalMultiplier : 1f;

            float progress = Mathf.InverseLerp(
                growthStartTime,
                lastWaveStartTime,
                currentWaveStartTime);
            return Mathf.Lerp(1f, finalMultiplier, progress);
        }

        private float GetWaveStartTime(int waveIndex)
        {
            float startTime = 0f;
            int endIndex = Mathf.Min(waveIndex, WaveList.Count);
            for (int i = 0; i < endIndex; i++)
            {
                WaveDataSO wave = WaveList[i];
                if (wave != null)
                    startTime += Mathf.Max(0, wave.ToNextWave);
            }

            return startTime;
        }
    }
}
