using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal class TutorialTrainingEnemies
    {
        private const float TrainingEnemyHealth = 5f;
        private readonly HashSet<Enemy> _tutorialEnemies = new();
        private readonly Spawner _spawner;
        private readonly TutorialSequenceController _settings;
        private readonly Player _player;
        private int _spawnIndex;

        public TutorialTrainingEnemies(Spawner spawner, TutorialSequenceController settings, Player player)
        {
            _spawner = spawner;
            _settings = settings;
            _player = player;
        }

        public bool IsReady => _spawner != null && _settings.TrainingEnemyPoolItem != null && _player != null;
        public int Count => _tutorialEnemies.Count;
        public bool Remove(Enemy enemy) => enemy != null && _tutorialEnemies.Remove(enemy);
        public void Refresh() => _tutorialEnemies.RemoveWhere(enemy => enemy == null || enemy.IsDead);

        public bool Spawn(float spawnDistance = -1f)
        {
            if (_spawner == null || _settings.TrainingEnemyPoolItem == null || _player == null)
                return false;

            Vector2[] directions =
            {
                Vector2.right,
                Vector2.up,
                Vector2.left,
                Vector2.down,
                new Vector2(1f, 1f).normalized,
                new Vector2(-1f, 1f).normalized,
                new Vector2(-1f, -1f).normalized,
                new Vector2(1f, -1f).normalized
            };
            Vector2 direction = directions[_spawnIndex++ % directions.Length];
            float distance = spawnDistance > 0f ? spawnDistance : _settings.EnemySpawnDistance;
            Vector2 spawnPosition = (Vector2)_player.transform.position + direction * distance;
            Enemy enemy = _spawner.SpawnSummonedAt(_settings.TrainingEnemyPoolItem, spawnPosition, _player as Entity);
            if (enemy == null)
                return false;

            ApplyTrainingHealth(enemy);
            _tutorialEnemies.Add(enemy);
            return true;
        }

        private static void ApplyTrainingHealth(Enemy enemy)
        {
            EntityStat stats = enemy.GetCompo<EntityStat>();
            if (stats == null || !stats.TryGetStatByName("Health", out var healthStat)) return;

            healthStat.BaseValue = TrainingEnemyHealth;
            enemy.GetComponentInChildren<EntityHealth>(true)?.ResetToStatHealth();
        }

        public void Clear()
        {
            foreach (Enemy enemy in _tutorialEnemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.IsDead)
                    enemy.ReturnToPool();
            }

            _tutorialEnemies.Clear();
        }
    }
}
