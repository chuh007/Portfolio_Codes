using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Item;
using _Work.CHUH.Code.WaveSystem;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;
using BossEnemy = _Work.CHUH.Code.Enemies.Boss.Boss;

namespace _Work.CHUH.Code.Enemies
{
    public class Spawner : MonoBehaviour, ITargetProvider, ITargetBufferProvider
    {
        [SerializeField] private float minDistance = 10;
        [SerializeField] private float maxDistance = 20;
        [SerializeField, Range(0f, 180f)] private float forwardRespawnAngle = 90f;
        [SerializeField, Range(0f, 1f)] private float spawnViewportMargin = 0.15f;
        [SerializeField, Range(0f, 2f)] private float despawnViewportMargin = 0.35f;

        [Header("Global Enemy Difficulty")]
        [SerializeField, Min(0.1f)] private float overallEnemyHealthMultiplier = 1.25f;

        [Header("Enemy Spawn Limits")]
        [SerializeField, Min(0f)] private float rangeEnemySpawnInterval = 10f;

        [Header("Boss Transition Performance")]
        [SerializeField, Min(0.1f)] private float bossCleanupFrameBudgetMilliseconds = 1f;
        [SerializeField, Min(1)] private int bossCleanupMaxEnemiesPerFrame = 8;
        
        [SerializeField] private PoolManagerSO poolManager;

        [Header("Item Chest")]
        [SerializeField] private PoolItemSO itemChestPoolItem;
        [SerializeField] private float minItemChestSpawnInterval = 15f;
        [SerializeField] private float maxItemChestSpawnInterval = 35f;
        [SerializeField] private float itemChestSpawnRadius = 15f;
        [SerializeField] private float itemChestMapPadding = 1f;
        [SerializeField, Min(0)] private int maxItemChestCount = 10;
        [SerializeField, Min(0f)] private float itemChestRespawnDistance = 20f;
        [SerializeField, Min(0f)] private float minItemChestRespawnRadius = 0f;
        [SerializeField, Min(0f)] private float maxItemChestRespawnRadius = 15f;
        [SerializeField] private List<WeightedPoolItem> itemChestDropTable = new List<WeightedPoolItem>();

        private readonly List<Enemy> _enemies = new List<Enemy>();
        private readonly List<ItemChest> _itemChests = new List<ItemChest>();
        private readonly EnemySpawnHealth _health = new EnemySpawnHealth();
        private EnemySpawnRuntime _runtime;
        private EnemySpawnRuntime Runtime => _runtime ??= new EnemySpawnRuntime(this, GetSettings, _enemies, _itemChests, _health);
        public int ActiveEnemyCount => _enemies.Count;
        public IReadOnlyList<Enemy> ActiveEnemies => _enemies;

        private void Awake() => Runtime.Initialize(this);
        private void Start() => Runtime.Start();
        private void Update() => Runtime.Tick();
        private void OnDestroy() => _runtime?.Dispose();

        public void SetWaveHealthMultiplierOverride(float multiplier, float finalHealthMultiplier = 1f)
            => _health.SetWaveHealthMultiplierOverride(multiplier, finalHealthMultiplier);

        public void ClearWaveHealthMultiplierOverride(float finalHealthMultiplier = 1f)
            => _health.ClearWaveHealthMultiplierOverride(finalHealthMultiplier);

        public void SpawnRandomEnemy(List<WaveEnemyData> enemyPool) => TrySpawnRandomEnemy(enemyPool);

        public bool TrySpawnRandomEnemy(List<WaveEnemyData> enemyPool, float spawnDistanceMultiplier = 1f)
            => Runtime.Factory.Selection.TrySpawnRandomEnemy(enemyPool, spawnDistanceMultiplier);

        public Enemy SpawnAt(PoolItemSO enemyItem, Vector2 spawnPos)
            => Runtime.Factory.SpawnAt(enemyItem, spawnPos, 1f, true);

        public BossEnemy ReplaceWithBossForm(Enemy outgoingEnemy, PoolItemSO bossPoolItem, Vector2 spawnPosition)
            => Runtime.Boss.ReplaceWithBossForm(outgoingEnemy, bossPoolItem, spawnPosition);

        public Enemy SpawnSummonedAt(PoolItemSO enemyItem, Vector2 spawnPos, Entity summonTarget)
        {
            Enemy summonedEnemy = Runtime.Factory.SpawnAt(
                enemyItem,
                spawnPos,
                1f,
                false,
                false);

            if (summonedEnemy != null && summonTarget != null)
                summonedEnemy.SetTarget(summonTarget);

            return summonedEnemy;
        }

        public Transform FindNearest(Vector3 from, float range)
        {
            return EnemyTargetFinder.FindNearest(_enemies, _itemChests, from, range);
        }

        public Transform[] FindAllInRange(Vector3 from, float range)
        {
            return EnemyTargetFinder.FindAllInRange(_enemies, _itemChests, from, range);
        }

        public void FillInRange(Vector3 from, float range, List<Transform> results)
        {
            results.Clear();
            EnemyTargetFinder.AddTargetsInRange(_enemies, _itemChests, from, range, results);
        }

        public Transform FindRandom(Vector3 from, float range)
        {
            return EnemyTargetFinder.FindRandom(_enemies, _itemChests, from, range);
        }

        private SpawnerSettings GetSettings()
        {
            return new SpawnerSettings
            {
                minDistance = minDistance,
                maxDistance = maxDistance,
                forwardRespawnAngle = forwardRespawnAngle,
                spawnViewportMargin = spawnViewportMargin,
                despawnViewportMargin = despawnViewportMargin,
                overallEnemyHealthMultiplier = overallEnemyHealthMultiplier,
                rangeEnemySpawnInterval = rangeEnemySpawnInterval,
                bossCleanupFrameBudgetMilliseconds = bossCleanupFrameBudgetMilliseconds,
                bossCleanupMaxEnemiesPerFrame = bossCleanupMaxEnemiesPerFrame,
                poolManager = poolManager,
                itemChestPoolItem = itemChestPoolItem,
                minItemChestSpawnInterval = minItemChestSpawnInterval,
                maxItemChestSpawnInterval = maxItemChestSpawnInterval,
                itemChestSpawnRadius = itemChestSpawnRadius,
                itemChestMapPadding = itemChestMapPadding,
                maxItemChestCount = maxItemChestCount,
                itemChestRespawnDistance = itemChestRespawnDistance,
                minItemChestRespawnRadius = minItemChestRespawnRadius,
                maxItemChestRespawnRadius = maxItemChestRespawnRadius,
                itemChestDropTable = itemChestDropTable
            };
        }
    }
}
