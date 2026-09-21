using System;
using _Work.CHUH.Code.Core;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.WaveSystem
{
    internal class WaveBossEncounter : IDisposable
    {
        private Enemy _activeBoss;
        public bool HasActiveBoss => _activeBoss != null;
        public bool IsSpawningStopped { get; private set; }
        public event Action OnEnemyKilled;
        public event Action OnSpawningStopped;
        public event Action OnEncounterStarted;
        public event Action OnEncounterFinished;

        public WaveBossEncounter()
        {
            Bus<EnemyDeadEvent>.OnEvent += HandleEnemyDead;
            Bus<BossSpawnedEvent>.OnEvent += HandleBossSpawned;
            Bus<BossPreludeSpawnedEvent>.OnEvent += HandleBossPreludeSpawned;
            Bus<Boss1EncounterStartedEvent>.OnEvent += HandleBoss1EncounterStarted;
        }

        public void Dispose()
        {
            Bus<EnemyDeadEvent>.OnEvent -= HandleEnemyDead;
            Bus<BossSpawnedEvent>.OnEvent -= HandleBossSpawned;
            Bus<BossPreludeSpawnedEvent>.OnEvent -= HandleBossPreludeSpawned;
            Bus<Boss1EncounterStartedEvent>.OnEvent -= HandleBoss1EncounterStarted;
        }

        private void HandleBossSpawned(BossSpawnedEvent evt)
        {
            if (evt.Boss != null) _activeBoss = evt.Boss;
            IsSpawningStopped = true;
            OnSpawningStopped?.Invoke();
        }

        private void HandleBossPreludeSpawned(BossPreludeSpawnedEvent evt)
        {
            if (evt.Boss != null) HandleBossSpawned(new BossSpawnedEvent(evt.Boss));
        }

        private void HandleEnemyDead(EnemyDeadEvent evt)
        {
            OnEnemyKilled?.Invoke();
            if (evt.Enemy != _activeBoss) return;
            _activeBoss = null;
            if (!IsSpawningStopped) OnEncounterFinished?.Invoke();
        }

        private void HandleBoss1EncounterStarted(Boss1EncounterStartedEvent evt)
        {
            if (evt.Boss == null) return;
            _activeBoss = evt.Boss;
            OnEncounterStarted?.Invoke();
        }
    }
}
