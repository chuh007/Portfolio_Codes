using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal readonly struct ActiveLargeDecoration
    {
        public ActiveLargeDecoration(Vector3Int cell, GameObject prefab, GameObject instance)
        {
            Cell = cell;
            Prefab = prefab;
            Instance = instance;
        }

        public Vector3Int Cell { get; }
        public GameObject Prefab { get; }
        public GameObject Instance { get; }
    }

    internal sealed class LargeDecorationChunks
    {
        private readonly Dictionary<Vector2Int, List<ActiveLargeDecoration>> _activeLargeDecorations = new();
        private readonly List<LargeDecorationPlacement> _placements = new();
        private readonly LargeDecorationPool _pool;

        public LargeDecorationChunks(LargeDecorationPool pool) => _pool = pool;

        public bool Add(Vector2Int coord, DecorationPlacementContext context)
        {
            context.GenerateLarge(coord, _placements);
            var active = new List<ActiveLargeDecoration>(_placements.Count);
            foreach (LargeDecorationPlacement placement in _placements)
            {
                GameObject instance = _pool.Acquire(placement.Prefab);
                _pool.ApplyTransform(instance, placement);
                active.Add(new ActiveLargeDecoration(placement.Cell, placement.Prefab, instance));
            }
            _activeLargeDecorations.Add(coord, active);
            return active.Count > 0;
        }

        public bool Remove(Vector2Int coord)
        {
            if (!_activeLargeDecorations.Remove(coord, out List<ActiveLargeDecoration> active)) return false;

            foreach (ActiveLargeDecoration decoration in active)
                _pool.Release(decoration);
            return active.Count > 0;
        }

        public void ClearArea(Rect worldArea)
        {
            foreach (List<ActiveLargeDecoration> activeLarge in _activeLargeDecorations.Values)
            {
                for (int i = activeLarge.Count - 1; i >= 0; i--)
                {
                    ActiveLargeDecoration decoration = activeLarge[i];
                    Vector2 position = new Vector2(
                        decoration.Cell.x + 0.5f,
                        decoration.Cell.y + 0.5f);
                    if (!worldArea.Contains(position))
                        continue;

                    _pool.Release(decoration);
                    activeLarge.RemoveAt(i);
                }
            }

            // The arena does not query these colliders immediately. Let the next
            // physics step apply the deactivations instead of globally syncing every
            // active 2D transform on the boss-spawn frame.
        }

        public void Clear()
        {
            foreach (List<ActiveLargeDecoration> active in _activeLargeDecorations.Values)
            {
                foreach (ActiveLargeDecoration decoration in active)
                    _pool.Release(decoration);
            }
            _activeLargeDecorations.Clear();
            _placements.Clear();
        }
    }
}
