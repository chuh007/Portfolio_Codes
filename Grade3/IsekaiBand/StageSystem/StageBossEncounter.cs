using System;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class StageBossEncounter
    {
        private const float BossArenaDecorationClearance = 2f;
        private readonly BossArenaController _bossArena;
        private readonly InfiniteMapDecorationRenderer _decorationRenderer;
        private readonly Func<Transform> _resolvePlayer;
        private readonly Func<Vector2> _readArenaSize;
        private readonly StageBossMusic _music;
        private Enemy _activeBossEncounter;

        public bool IsActive => _activeBossEncounter != null;

        public StageBossEncounter(BossArenaController bossArena, InfiniteMapDecorationRenderer decorations,
            Func<Transform> resolvePlayer, Func<Vector2> readArenaSize, StageBossMusic music)
        {
            _bossArena = bossArena;
            _decorationRenderer = decorations;
            _resolvePlayer = resolvePlayer;
            _readArenaSize = readArenaSize;
            _music = music;
            Bus<BossSpawnedEvent>.OnEvent += HandleBossSpawned;
            Bus<BossPreludeSpawnedEvent>.OnEvent += HandleBossPreludeSpawned;
            Bus<Boss1EncounterStartedEvent>.OnEvent += HandleBoss1EncounterStarted;
            Bus<EnemyDeadEvent>.OnEvent += HandleEnemyDead;
        }

        public void Dispose()
        {
            Bus<BossSpawnedEvent>.OnEvent -= HandleBossSpawned;
            Bus<BossPreludeSpawnedEvent>.OnEvent -= HandleBossPreludeSpawned;
            Bus<Boss1EncounterStartedEvent>.OnEvent -= HandleBoss1EncounterStarted;
            Bus<EnemyDeadEvent>.OnEvent -= HandleEnemyDead;
            _music.StopOnDestroy();
        }

        private void HandleBossSpawned(BossSpawnedEvent evt) => BeginBossEncounter(evt.Boss);
        private void HandleBossPreludeSpawned(BossPreludeSpawnedEvent evt) => BeginBossEncounter(evt.Boss);

        private void HandleBoss1EncounterStarted(Boss1EncounterStartedEvent evt)
        {
            if (evt.Boss == null)
                return;

            _music.Play(evt.Boss);
            BeginBossEncounter(evt.Boss);
        }

        private void BeginBossEncounter(Enemy boss)
        {
            if (boss == null)
                return;

            _activeBossEncounter = boss;
            EnsureBossArena(boss);
        }

        private void EnsureBossArena(Enemy boss)
        {
            if (_bossArena == null || boss == null)
                return;

            if (_bossArena.IsActive)
            {
                _bossArena.ReplaceBoss(boss);
                return;
            }

            Transform playerTransform = _resolvePlayer();
            if (playerTransform == null)
                return;

            Vector2 clearSize = _readArenaSize() + Vector2.one * (BossArenaDecorationClearance * 2f);
            Vector2 clearMin = (Vector2)playerTransform.position - clearSize * 0.5f;
            _decorationRenderer?.ClearLargeDecorations(new Rect(clearMin, clearSize));
            _bossArena.Create(boss, playerTransform.position);
        }

        private void HandleEnemyDead(EnemyDeadEvent evt)
        {
            _music.HandleEnemyDead(evt);

            if (evt.Enemy == _activeBossEncounter)
                _activeBossEncounter = null;

            if (_bossArena == null || evt.Enemy != _bossArena.Boss)
                return;

            _bossArena.Deactivate();
        }
    }
}
