using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Core;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.Item;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    internal class EnemySpawnRuntime
    {
        private readonly Func<SpawnerSettings> _getSettings;
        private readonly List<Enemy> _enemies;
        private readonly List<ItemChest> _itemChests;
        private readonly SpawnerPositionTracker _positions;
        private float _itemChestSpawnTimer;
        public EnemySpawnFactory Factory { get; }
        public BossSpawnHandler Boss { get; }

        public EnemySpawnRuntime(Spawner owner, Func<SpawnerSettings> getSettings,
            List<Enemy> enemies, List<ItemChest> chests, EnemySpawnHealth health)
        {
            _getSettings = getSettings;
            _enemies = enemies;
            _itemChests = chests;
            _positions = new SpawnerPositionTracker(getSettings);
            Factory = new EnemySpawnFactory(getSettings, enemies, _positions, health);
            var cleanup = new EnemySpawnCleanup(owner, enemies,
                () => getSettings().bossCleanupFrameBudgetMilliseconds,
                () => getSettings().bossCleanupMaxEnemiesPerFrame);
            Boss = new BossSpawnHandler(Factory, health, cleanup, enemies);
        }

        public void Initialize(ITargetProvider targetProvider)
        {
            _positions.Initialize();
            Factory.Initialize();
            Bus<EnemyDeadEvent>.OnEvent += HandleEnemyDead;
            Bus<MapSizeSetEvent>.OnEvent += _positions.HandleMapSize;
            Bus<EnemySpawnEvent>.OnEvent += Boss.HandleEnemySpawn;
            TargetingService.SetProvider(targetProvider);
            _itemChestSpawnTimer = ItemChestSpawner.NextSpawnInterval(
                _getSettings().minItemChestSpawnInterval, _getSettings().maxItemChestSpawnInterval);
        }

        public void Start() => _positions.CacheCameraSize();

        public void Dispose()
        {
            Bus<EnemyDeadEvent>.OnEvent -= HandleEnemyDead;
            Bus<MapSizeSetEvent>.OnEvent -= _positions.HandleMapSize;
            Bus<EnemySpawnEvent>.OnEvent -= Boss.HandleEnemySpawn;
        }

        public void Tick()
        {
            if (Time.timeScale <= 0f)
                return;

            _positions.ResolvePlayer();
            SpawnerPositionContext positionContext = _positions.CreatePositionContext();
            EnemyRepositioner.RepositionFarEnemies(
                _enemies,
                _positions.Player,
                _getSettings().poolManager,
                positionContext,
                (poolItem, spawnPosition) => Factory.SpawnAt(
                    poolItem,
                    spawnPosition,
                    1f,
                    true,
                    false,
                    SpawnPositionMode.FindOffscreen));

            if (StageHelper.Instance?.IsBossArenaActive != true)
            {
                _itemChestSpawnTimer = ItemChestSpawner.TickTimer(
                    _itemChestSpawnTimer,
                    Time.deltaTime,
                    _getSettings().poolManager,
                    _getSettings().itemChestPoolItem,
                    _itemChests,
                    _getSettings().itemChestDropTable,
                    positionContext,
                    _getSettings().itemChestSpawnRadius,
                    _getSettings().itemChestMapPadding,
                    _getSettings().maxItemChestCount,
                    _getSettings().itemChestRespawnDistance,
                    _getSettings().minItemChestRespawnRadius,
                    _getSettings().maxItemChestRespawnRadius,
                    _getSettings().minItemChestSpawnInterval,
                    _getSettings().maxItemChestSpawnInterval);
            }
        }

        private void HandleEnemyDead(EnemyDeadEvent evt)
        {
            _enemies.Remove(evt.Enemy);
        }
    }
}
