using System.Collections.Generic;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.EntityPlus;
using _Work.CHUH.Code.Enemies.Boss;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using BossEnemy = _Work.CHUH.Code.Enemies.Boss.Boss;

namespace _Work.CHUH.Code.Enemies
{
    internal class BossSpawnHandler
    {
        private const string Boss1PoolName = "Boss1";
        private readonly EnemySpawnFactory _factory;
        private readonly EnemySpawnHealth _health;
        private readonly EnemySpawnCleanup _cleanup;
        private readonly List<Enemy> _enemies;

        public BossSpawnHandler(EnemySpawnFactory factory, EnemySpawnHealth health,
            EnemySpawnCleanup cleanup, List<Enemy> enemies)
        {
            _factory = factory;
            _health = health;
            _cleanup = cleanup;
            _enemies = enemies;
        }

        public void HandleEnemySpawn(EnemySpawnEvent evt)
        {
            Enemy spawnedEnemy = _factory.Spawn(evt.PoolItem, 1f, false);
            if (spawnedEnemy == null)
                return;

            _health.ApplyMiddleBossHealthScaling(spawnedEnemy);

            EntityHealth spawnedHealth = spawnedEnemy.GetComponentInChildren<EntityHealth>(true);
            spawnedHealth?.SetBossDamageAttenuationEnabled(evt.UseBossDamageAttenuation);

            if (evt.RewardCount > 0)
                spawnedEnemy.SetRewardCount(evt.RewardCount);

            PianoBossHumanFormTransition prelude =
                spawnedEnemy.GetComponent<PianoBossHumanFormTransition>();
            if (prelude != null && prelude.IsConfigured)
            {
                prelude.BeginEncounter(evt.RewardCount);
                _cleanup.ClearEnemiesExcept(spawnedEnemy);
                Bus<BossPreludeSpawnedEvent>.Raise(new BossPreludeSpawnedEvent(spawnedEnemy));
                return;
            }

            bool isBoss1 = IsBoss1(evt.PoolItem);
            if (isBoss1
                && spawnedEnemy is BossEnemy middleBoss
                && middleBoss.TreatAsMiddleBoss)
            {
                _cleanup.ClearEnemiesExcept(spawnedEnemy, playDeathAnimation: true);
                Bus<Boss1EncounterStartedEvent>.Raise(new Boss1EncounterStartedEvent(spawnedEnemy));
                return;
            }

            if (spawnedEnemy is not BossEnemy boss || boss.TreatAsMiddleBoss)
                return;

            _cleanup.ClearEnemiesExcept(spawnedEnemy, playDeathAnimation: isBoss1);
            Bus<BossSpawnedEvent>.Raise(new BossSpawnedEvent(spawnedEnemy));
        }

        private static bool IsBoss1(PoolItemSO poolItem)
        {
            return poolItem != null
                   && (poolItem.poolingName == Boss1PoolName || poolItem.name == Boss1PoolName);
        }

        public BossEnemy ReplaceWithBossForm(
            Enemy outgoingEnemy,
            PoolItemSO bossPoolItem,
            Vector2 spawnPosition)
        {
            bool useBossDamageAttenuation = outgoingEnemy != null
                && outgoingEnemy.GetComponentInChildren<EntityHealth>(true)
                    is { IsBossDamageAttenuationEnabled: true };
            StageHelper stageHelper = StageHelper.Instance;
            bool hasActiveBossArena = stageHelper != null && stageHelper.IsBossArenaActive;
            if (hasActiveBossArena)
                spawnPosition = stageHelper.BossArenaCenter;

            Enemy spawnedEnemy = _factory.SpawnAt(
                bossPoolItem,
                spawnPosition,
                1f,
                false,
                false,
                hasActiveBossArena
                    ? SpawnPositionMode.UseExact
                    : SpawnPositionMode.FindUnblocked);
            if (spawnedEnemy is not BossEnemy boss)
            {
                if (spawnedEnemy != null)
                {
                    Debug.LogWarning($"[Spawner] {bossPoolItem.name} prefab needs Boss component.");
                    _enemies.Remove(spawnedEnemy);
                    spawnedEnemy.ReturnToPool();
                }

                return null;
            }

            boss.GetComponentInChildren<EntityHealth>(true)
                ?.SetBossDamageAttenuationEnabled(useBossDamageAttenuation);

            if (outgoingEnemy != null)
            {
                _enemies.Remove(outgoingEnemy);
                if (outgoingEnemy.gameObject.activeInHierarchy)
                    outgoingEnemy.ReturnToPool();
            }

            return boss;
        }

    }
}
