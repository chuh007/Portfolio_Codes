using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

namespace _Work.CHUH.Code.Enemies
{
    internal class EnemyCrowdCache
    {
        private static readonly ProfilerMarker ApplyMarker = new("EnemyCrowdSeparation.Apply");
        public List<Enemy> Agents { get; } = new();
        public List<Rigidbody2D> Bodies { get; } = new();
        public List<int> LifecycleVersions { get; } = new();
        public List<Vector2> Positions { get; } = new();
        public List<Vector2Int> Cells { get; } = new();
        public List<Vector2> Velocities { get; } = new();
        public EnemyCrowdGrid Grid { get; } = new();

        public void Refresh(IReadOnlyList<Enemy> activeEnemies, float cellSize)
        {
            Clear();
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                Enemy enemy = activeEnemies[i];
                if (!CanSeparate(enemy))
                    continue;

                Rigidbody2D body = enemy.PhysicsBody;
                Vector2 position = body.position;
                Vector2Int cell = EnemyCrowdGrid.GetCell(position, cellSize);
                int agentIndex = Agents.Count;

                Agents.Add(enemy);
                Bodies.Add(body);
                LifecycleVersions.Add(enemy.PoolLifecycleVersion);
                Positions.Add(position);
                Cells.Add(cell);
                Velocities.Add(Vector2.zero);
                Grid.GetOrCreateBucket(cell).Add(agentIndex);
            }
        }

        public void Apply()
        {
            using ProfilerMarker.AutoScope _ = ApplyMarker.Auto();
            for (int i = 0; i < Agents.Count; i++)
            {
                Enemy enemy = Agents[i];
                Rigidbody2D body = Bodies[i];
                if (!CanSeparate(enemy)
                    || body == null
                    || enemy.PoolLifecycleVersion != LifecycleVersions[i])
                    continue;

                body.linearVelocity += Velocities[i];
            }
        }

        private static bool CanSeparate(Enemy enemy)
        {
            if (enemy == null
                || enemy.IsBoss
                || enemy.IsDead
                || !enemy.gameObject.activeInHierarchy)
                return false;

            Rigidbody2D body = enemy.PhysicsBody;
            return body != null && body.simulated;
        }

        private void Clear()
        {
            Grid.Clear();
            Agents.Clear();
            Bodies.Clear();
            LifecycleVersions.Clear();
            Positions.Clear();
            Cells.Clear();
            Velocities.Clear();
        }
    }
}
