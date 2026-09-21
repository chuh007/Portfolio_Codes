using System;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WaveSystem
{
    internal class WaveSpawnController : IDisposable
    {
        private readonly Spawner _spawner;
        private readonly WaveBossEncounter _encounter;
        private readonly WaveMinimumFill _fill;
        private readonly Func<float> _getSpawnRateMultiplier;
        private readonly Func<WaveFillSettings> _getFillSettings;
        private readonly Func<float> _getDifficulty;
        private WaveDataSO _wave;
        private StageDataSO _stage;
        private int _waveIndex = -1;
        private double _spawnTime;

        public bool CanSpawn => !_encounter.IsSpawningStopped && !_encounter.HasActiveBoss && _wave != null;
        public int ActiveCount => _spawner.ActiveEnemyCount;
        public int MinimumCount => _wave.MinEnemiesCount;
        public int MaximumCount => _wave.MaxEnemiesCount;
        public double SpawnInterval => Math.Max(0.001d,
            _wave.SpawnInterval / Mathf.Max(0.1f, _getSpawnRateMultiplier()));

        public WaveSpawnController(Spawner spawner, WaveBossEncounter encounter,
            Func<float> getSpawnRateMultiplier, Func<WaveFillSettings> getFillSettings, Func<float> getDifficulty)
        {
            _spawner = spawner;
            _encounter = encounter;
            _getSpawnRateMultiplier = getSpawnRateMultiplier;
            _getFillSettings = getFillSettings;
            _getDifficulty = getDifficulty;
            _fill = new WaveMinimumFill(this);
            Bus<WaveChangeEvent>.OnEvent += HandleWaveChange;
            _encounter.OnSpawningStopped += StopWave;
            _encounter.OnEncounterStarted += _fill.Dispose;
            _encounter.OnEncounterFinished += StartRecovery;
        }

        public void Dispose()
        {
            Bus<WaveChangeEvent>.OnEvent -= HandleWaveChange;
            _encounter.OnSpawningStopped -= StopWave;
            _encounter.OnEncounterStarted -= _fill.Dispose;
            _encounter.OnEncounterFinished -= StartRecovery;
            _fill.Dispose();
        }

        private void HandleWaveChange(WaveChangeEvent evt)
        {
            if (_encounter.IsSpawningStopped) return;
            _wave = evt.WaveData;
            if (evt.StageData != null) _stage = evt.StageData;
            _waveIndex = evt.WaveIndex >= 0 ? evt.WaveIndex : _waveIndex + 1;
            WaveDifficultyTuning.ApplyHealth(_spawner, _stage, _wave, _waveIndex, _getDifficulty());
            _spawnTime = 0f;
            StartFill();
        }

        public void Tick()
        {
            if (!CanSpawn) return;
            if (!_fill.IsFilling && ActiveCount < MinimumCount) StartFill();
            if (_fill.IsFilling) return;
            int maximum = MaximumCount;
            if (ActiveCount >= maximum) return;
            _spawnTime += Time.deltaTime;
            double interval = SpawnInterval;
            while (_spawnTime >= interval)
            {
                _spawnTime -= interval;
                _spawner.SpawnRandomEnemy(_wave.EnemyPool);
                if (ActiveCount >= maximum) break;
            }
        }

        public bool TrySpawn(float distance) => _spawner.TrySpawnRandomEnemy(_wave.EnemyPool, distance);

        private void StartFill()
        {
            if (Time.timeScale <= 0f || _encounter.IsSpawningStopped || _encounter.HasActiveBoss) return;
            WaveFillSettings settings = _getFillSettings();
            _fill.Begin(settings.Duration, 1f, settings.BatchSize);
        }

        private void StartRecovery()
        {
            if (_wave == null) return;
            _spawnTime = 0f;
            WaveFillSettings settings = _getFillSettings();
            _fill.Begin(settings.RecoveryDuration, settings.RecoveryDistance, settings.BatchSize);
        }

        private void StopWave()
        {
            _wave = null;
            _spawnTime = 0f;
            _fill.Dispose();
        }
    }
}
