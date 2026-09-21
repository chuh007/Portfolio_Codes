using System;
using TMPro;
using UnityEngine;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.Test
{
    public class TestTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;

        private float _currentTime;
        private Enemy _activeBossEncounter;
        private bool _wasTimerTextEnabled;

        private void OnEnable()
        {
            Bus<BossSpawnedEvent>.OnEvent += HandleBossSpawned;
            Bus<BossPreludeSpawnedEvent>.OnEvent += HandleBossPreludeSpawned;
            Bus<Boss1EncounterStartedEvent>.OnEvent += HandleBoss1EncounterStarted;
            Bus<EnemyDeadEvent>.OnEvent += HandleEnemyDead;
        }

        private void OnDisable()
        {
            Bus<BossSpawnedEvent>.OnEvent -= HandleBossSpawned;
            Bus<BossPreludeSpawnedEvent>.OnEvent -= HandleBossPreludeSpawned;
            Bus<Boss1EncounterStartedEvent>.OnEvent -= HandleBoss1EncounterStarted;
            Bus<EnemyDeadEvent>.OnEvent -= HandleEnemyDead;
        }

        private void Update()
        {
            if (_activeBossEncounter != null)
                return;

            _currentTime += Time.deltaTime * (TestDoubleMode.Instance.isOnDoubleMode ? 2f : 1f);
            int minutes = (int)(_currentTime / 60);
            int seconds = (int)(_currentTime % 60);
            timerText.SetText($"{minutes:00}:{seconds:00}");
        }

        private void HandleBossSpawned(BossSpawnedEvent evt)
        {
            BeginBossEncounter(evt.Boss);
        }

        private void HandleBossPreludeSpawned(BossPreludeSpawnedEvent evt)
        {
            BeginBossEncounter(evt.Boss);
        }

        private void HandleBoss1EncounterStarted(Boss1EncounterStartedEvent evt)
        {
            BeginBossEncounter(evt.Boss);
        }

        private void HandleEnemyDead(EnemyDeadEvent evt)
        {
            if (evt.Enemy != _activeBossEncounter)
                return;

            _activeBossEncounter = null;
            if (timerText != null)
                timerText.enabled = _wasTimerTextEnabled;
        }

        private void BeginBossEncounter(Enemy boss)
        {
            if (boss == null)
                return;

            if (_activeBossEncounter == null && timerText != null)
                _wasTimerTextEnabled = timerText.enabled;

            _activeBossEncounter = boss;
            if (timerText != null)
                timerText.enabled = false;
        }
    }
}
