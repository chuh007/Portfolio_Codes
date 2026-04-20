using System;
using Chuh007Lib.Dependencies;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Work.CHUH._01Scripts.Combat;
using Random = UnityEngine.Random;

namespace Work.CHUH._01Scripts.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public UnityEvent enemyAllKillEvent;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private BossListSO bossListSO;

        [Header("Temp")]
        [SerializeField] private float flyEnemyUpValue = 5f;
        [SerializeField] private Transform spawnTrm;
 
        [Inject] private Castle castle;
        
        [field: SerializeField] public bool IsSpawning { get; private set; } = false;
        public void SetSpawnStatus(bool status) => IsSpawning = status;

        public int leftEnemyCount;
        
        public void SpawnEnemies(PoolItemSO enemyPoolItem, int count, int waveCnt)
        {
            int scaled = count + waveCnt / 5;
            leftEnemyCount += scaled;
            for (int i = 0; i < scaled; ++i)
            {
                Enemy enemy = poolManager.Pop(enemyPoolItem) as Enemy;
                SpawnEnemy(waveCnt, enemy);
            }
        }

        public void SpawnRandomBoss(int waveCnt)
        {
            leftEnemyCount++;
            var bossPool = bossListSO.bossPoolSOList[Random.Range(0, bossListSO.bossPoolSOList.Count)];
            Enemy enemy = poolManager.Pop(bossPool) as Enemy;
            SpawnEnemy(waveCnt, enemy);
        }

        private void SpawnEnemy(int waveCnt, Enemy enemy)
        {
            enemy.MultiplyStat("Damage", waveCnt);
            enemy.MultiplyStat("HP", waveCnt);
            enemy.transform.position = spawnTrm.position + (Vector3)Random.insideUnitCircle * 1f;
            enemy.SetTarget(castle);
            if (enemy.IsFlying) enemy.transform.position += Vector3.up * flyEnemyUpValue;
            enemy.OnDeadEvent.AddListener(HandleEnemyDead);
        }

        private void HandleEnemyDead()
        {
            leftEnemyCount--;
            if (leftEnemyCount <= 0 && !IsSpawning)
            {
                enemyAllKillEvent?.Invoke();
            }
        }

        public void ClearEnemies()
        {
            leftEnemyCount = 0;
        }
    }
}