using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

namespace _Work.CHUH.Code.Enemies
{
    internal static class EnemyCrowdVelocity
    {
        private const float MinimumSqrDistance = 0.0001f;

        public static void Calculate(EnemyCrowdCache cache, float radius, int maxNeighbors, float separationSpeed)
        {
            float sqrRadius = radius * radius;
            int neighborLimit = Mathf.Max(1, maxNeighbors);
            float speed = Mathf.Max(0f, separationSpeed);

            for (int agentIndex = 0; agentIndex < cache.Agents.Count; agentIndex++)
            {
                Vector2 position = cache.Positions[agentIndex];
                Vector2Int cell = cache.Cells[agentIndex];
                Vector2 separation = Vector2.zero;
                int neighborCount = 0;

                for (int y = -1; y <= 1 && neighborCount < neighborLimit; y++)
                {
                    for (int x = -1; x <= 1 && neighborCount < neighborLimit; x++)
                    {
                        if (!cache.Grid.TryGetBucket(cell.x + x, cell.y + y, out List<int> bucket))
                            continue;

                        for (int bucketIndex = 0;
                             bucketIndex < bucket.Count && neighborCount < neighborLimit;
                             bucketIndex++)
                        {
                            int otherIndex = bucket[bucketIndex];
                            if (otherIndex == agentIndex)
                                continue;

                            Vector2 delta = position - cache.Positions[otherIndex];
                            float sqrDistance = delta.sqrMagnitude;
                            if (sqrDistance >= sqrRadius)
                                continue;

                            Vector2 direction;
                            float distance;
                            if (sqrDistance <= MinimumSqrDistance)
                            {
                                direction = GetStableOverlapDirection(
                                    cache.Agents[agentIndex].GetInstanceID(),
                                    cache.Agents[otherIndex].GetInstanceID());
                                distance = 0f;
                            }
                            else
                            {
                                distance = Mathf.Sqrt(sqrDistance);
                                direction = delta / distance;
                            }

                            float proximity = 1f - distance / radius;
                            separation += direction * proximity;
                            neighborCount++;
                        }
                    }
                }

                cache.Velocities[agentIndex] = Vector2.ClampMagnitude(separation, 1f) * speed;
            }
        }

        private static Vector2 GetStableOverlapDirection(int firstId, int secondId)
        {
            int lowerId = Mathf.Min(firstId, secondId);
            int upperId = Mathf.Max(firstId, secondId);
            uint pairHash = unchecked((uint)(lowerId * 397) ^ (uint)upperId);
            float angle = (pairHash & 1023u) * (Mathf.PI * 2f / 1024f);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return firstId <= secondId ? direction : -direction;
        }
    }
}
