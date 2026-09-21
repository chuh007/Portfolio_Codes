using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class InfiniteMapVisibleChunks
    {
        private readonly HashSet<Vector2Int> _active = new();
        private readonly HashSet<Vector2Int> _needed = new();
        private readonly List<Vector2Int> _unused = new();
        private Vector2Int _center;
        private int _radius;
        private bool _isComplete;

        public void Clear()
        {
            _active.Clear();
            _needed.Clear();
            _unused.Clear();
            _isComplete = false;
        }

        public bool Refresh(Vector2Int center, int radius, Func<Vector2Int, bool> add, Action<Vector2Int> remove)
        {
            if (_isComplete && _center == center && _radius == radius)
                return false;

            _isComplete = false;
            _needed.Clear();
            _unused.Clear();
            bool changed = false;
            bool complete = true;
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    var coord = new Vector2Int(center.x + x, center.y + y);
                    _needed.Add(coord);
                    if (_active.Contains(coord)) continue;
                    if (!add(coord))
                    {
                        complete = false;
                        continue;
                    }

                    _active.Add(coord);
                    changed = true;
                }
            }

            foreach (Vector2Int coord in _active)
            {
                if (!_needed.Contains(coord))
                    _unused.Add(coord);
            }
            foreach (Vector2Int coord in _unused)
            {
                remove(coord);
                _active.Remove(coord);
                changed = true;
            }
            _center = center;
            _radius = radius;
            // Retry unavailable chunks even when the player has not moved.
            _isComplete = complete;
            return changed;
        }
    }
}
