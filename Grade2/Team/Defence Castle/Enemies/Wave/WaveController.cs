using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Chipmunk.GameEvents;
using Chuh007Lib.Dependencies;
using UnityEngine;
using UnityEngine.Events;
using Work.Chipmunk._01.Scripts.Networking;
using Work.Chipmunk._01.Scripts.UI;
using Work.CHUH._01Scripts.Enemies.Wave.GameEvents;
using Random = UnityEngine.Random;

namespace Work.CHUH._01Scripts.Enemies.Wave
{
    public class WaveController : MonoBehaviour
    {
        [SerializeField] private WaveDataSO waveData;
        [SerializeField] private EnemySpawner spawner;

        [field: SerializeField] public int WaveCount { get; private set; } = 1;

        [SerializeField] private float waveDelay;
        [SerializeField] private int bossWaveCount;
        [SerializeField] private int level = 1;

        private WaveSO _currentWave;
        private CancellationTokenSource _tokenSource;

        [Inject] private WaveStartButton _waveStartButton;
        [Inject] private WaveResultUI _waveResultUI;
        [SerializeField] private LeaderboardSO waveRanking;

        [ContextMenu("Test")]
        public void Text()
        {
            StartWave();
        }

        private void Awake()
        {
            spawner.enemyAllKillEvent.AddListener(HandleEnemyClear);
            EventBus<WaveEndEvent>.OnEvent += HandleWaveEnd;
            
            _waveStartButton.EnableFor(StartWave);
        }


        private void HandleWaveEnd(WaveEndEvent evt)
        {
            if (evt.EndReason == WaveEndEvent.Reason.AllEnemiesDefeated)
            {
                waveRanking.AddScoreAsync(WaveCount);
            }

            _waveResultUI.EnableFor(evt, this);
            if (evt.EndReason == WaveEndEvent.Reason.AllEnemiesDefeated) return;
            // 승리가 아닌 다른 요인으로 인한 종료(패배)
            _tokenSource?.Cancel();
            spawner.ClearEnemies();
            spawner.SetSpawnStatus(false);
        }

        private void HandleEnemyClear()
        {
            WaveCount++;
            EventBus.Raise(new WaveEndEvent(WaveEndEvent.Reason.AllEnemiesDefeated));
        }

        public void StartWave()
        {
            EventBus.Raise(new WaveStartEvent(WaveCount));

            _tokenSource = new CancellationTokenSource();
            level = WaveCount / 10 + 1;
            _currentWave = GetRandomWaveType();
            if (_tokenSource.IsCancellationRequested) return;
            _ = WaveCall();
        }

        private async Task WaveCall()
        {
            spawner.SetSpawnStatus(true);
            foreach (var data in _currentWave.enemies)
            {
                if (_tokenSource.IsCancellationRequested) return;
                spawner.SpawnEnemies(data.enemyPoolSO, data.baseSpawnCnt, WaveCount - 1);
                await Awaitable.WaitForSecondsAsync(data.delayToNextEnemy, _tokenSource.Token);
            }
            
            if (WaveCount % bossWaveCount == 0)
            {
                spawner.SpawnRandomBoss(WaveCount);
            }
            spawner.SetSpawnStatus(false);
        }

        private WaveSO GetRandomWaveType()
            => waveData.waveList[
                Random.Range(Mathf.Min(level * 2 - 2, waveData.waveList.Count - 1),
                    Mathf.Min((level + 1) * 3, waveData.waveList.Count))];
    }
}