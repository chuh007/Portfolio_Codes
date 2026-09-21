using System;
using _Work.CHUH.Code.Enemies;
using UnityEngine;

namespace _Work.CHUH.Code.WaveSystem
{
    public class WaveController : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        [Header("Enemy Spawn Timing")]
        [SerializeField, Min(0.1f)] private float enemySpawnRateMultiplier = 1.9440002f;
        [SerializeField, Min(1)] private int minimumFillSpawnBatchSize = 8;
        [SerializeField, Min(0.1f)] private float minimumFillDuration = 5f;
        [Header("Weapon Progression Difficulty")]
        [SerializeField, Range(0f, 2f)] private float weaponProgressionDifficulty = 1f;
        [Header("Boss1 Recovery Spawning")]
        [SerializeField, Min(0.1f)] private float boss1RecoveryDuration = 5f;
        [SerializeField, Min(1f)] private float boss1RecoverySpawnDistanceMultiplier = 1.5f;

        private WaveBossEncounter _encounter;
        private WaveSpawnController _spawning;
        public event Action<int> OnKillCountChanged;
        public int KillCount { get; private set; }
        public float ElapsedTime { get; private set; }

        private void Awake()
        {
            _encounter = new WaveBossEncounter();
            _encounter.OnEnemyKilled += HandleEnemyKilled;
            _spawning = new WaveSpawnController(spawner, _encounter,
                () => enemySpawnRateMultiplier, GetFillSettings, () => weaponProgressionDifficulty);
        }

        private void OnDestroy()
        {
            _spawning.Dispose();
            _encounter.OnEnemyKilled -= HandleEnemyKilled;
            _encounter.Dispose();
        }

        private void Update()
        {
            if (Time.timeScale <= 0f || _encounter.HasActiveBoss) return;
            ElapsedTime += Time.deltaTime;
            _spawning.Tick();
        }

        private void HandleEnemyKilled()
        {
            KillCount++;
            OnKillCountChanged?.Invoke(KillCount);
        }

        private WaveFillSettings GetFillSettings()
        {
            return new WaveFillSettings
            {
                Duration = minimumFillDuration,
                BatchSize = minimumFillSpawnBatchSize,
                RecoveryDuration = boss1RecoveryDuration,
                RecoveryDistance = boss1RecoverySpawnDistanceMultiplier
            };
        }
    }
}
