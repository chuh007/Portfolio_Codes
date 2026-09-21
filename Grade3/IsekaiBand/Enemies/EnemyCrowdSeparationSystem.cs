using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

namespace _Work.CHUH.Code.Enemies
{
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Spawner))]
    public sealed class EnemyCrowdSeparationSystem : MonoBehaviour
    {
        private const string EnemyLayerName = "Enemy";
        private static readonly ProfilerMarker RecalculateMarker = new("EnemyCrowdSeparation.Recalculate");

        [SerializeField] private Spawner spawner;
        [SerializeField, Min(0.1f)] private float separationRadius = 0.65f;
        [SerializeField, Min(0f)] private float separationSpeed = 1.5f;
        [SerializeField, Min(0.02f)] private float calculationInterval = 0.08f;
        [SerializeField, Min(1)] private int maxNeighbors = 12;

        private readonly EnemyCrowdCache _cache = new();
        private int _enemyLayer;
        private float _calculationTimer;

        private void Awake()
        {
            if (spawner == null)
                spawner = GetComponent<Spawner>();

            if (spawner == null)
            {
                Debug.LogError($"{nameof(EnemyCrowdSeparationSystem)} needs a {nameof(Spawner)}.", this);
                enabled = false;
                return;
            }

            _enemyLayer = LayerMask.NameToLayer(EnemyLayerName);
            if (_enemyLayer < 0)
            {
                Debug.LogError($"{nameof(EnemyCrowdSeparationSystem)} needs an '{EnemyLayerName}' layer.", this);
                enabled = false;
                return;
            }

            // 적끼리의 접촉은 군집 계산으로 처리하고 벽·플레이어와의 충돌은 유지한다.
            Physics2D.IgnoreLayerCollision(_enemyLayer, _enemyLayer, true);
        }

        private void FixedUpdate()
        {
            if (spawner == null || separationSpeed <= 0f)
                return;

            _calculationTimer -= Time.fixedDeltaTime;
            if (_calculationTimer <= 0f)
            {
                RecalculateSeparation();
                _calculationTimer = Mathf.Max(0.02f, calculationInterval);
            }

            _cache.Apply();
        }

        private void RecalculateSeparation()
        {
            using ProfilerMarker.AutoScope _ = RecalculateMarker.Auto();
            float radius = Mathf.Max(0.1f, separationRadius);
            _cache.Refresh(spawner.ActiveEnemies, radius);
            EnemyCrowdVelocity.Calculate(_cache, radius, maxNeighbors, separationSpeed);
        }
    }
}
