using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

namespace _Work.CHUH.Code.Enemies
{
    internal class EnemyCrowdGrid
    {
        private readonly Dictionary<long, List<int>> _grid = new();
        private readonly Stack<List<int>> _bucketPool = new();

        public bool TryGetBucket(int x, int y, out List<int> bucket)
            => _grid.TryGetValue(GetCellKey(x, y), out bucket);

        public void Clear()
        {
            foreach (List<int> bucket in _grid.Values)
            {
                bucket.Clear();
                _bucketPool.Push(bucket);
            }
            _grid.Clear();
        }

        public List<int> GetOrCreateBucket(Vector2Int cell)
        {
            long key = GetCellKey(cell.x, cell.y);
            if (_grid.TryGetValue(key, out List<int> bucket))
                return bucket;

            bucket = _bucketPool.Count > 0 ? _bucketPool.Pop() : new List<int>(8);
            _grid.Add(key, bucket);
            return bucket;
        }

        public static Vector2Int GetCell(Vector2 position, float cellSize)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / cellSize),
                Mathf.FloorToInt(position.y / cellSize));
        }

        public static long GetCellKey(int x, int y)
        {
            return ((long)x << 32) | (uint)y;
        }
    }
}
