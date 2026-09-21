using System;
using _Work.CHUH.Code.Core.Events;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem
{
    public class StageHelper : MonoBehaviour
    {
        public static StageHelper Instance;
        
        [HideInInspector] public float mapHalfWidth;
        [HideInInspector] public float mapHalfHeight;
        [HideInInspector] public bool isInfiniteMap;

        private Rect _bossArenaBounds;
        private bool _isBossArenaActive;

        public bool IsBossArenaActive => _isBossArenaActive;
        public Vector2 BossArenaCenter => _bossArenaBounds.center;
        public Vector2 BossArenaSize => _bossArenaBounds.size;

        private void Awake()
        {
            Bus<MapSizeSetEvent>.OnEvent += HandleMapSizeSet;
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Bus<MapSizeSetEvent>.OnEvent -= HandleMapSizeSet;
        }

        public bool CheckMapBound(Vector2 pos, float padding)
        {
            if (_isBossArenaActive)
                return Contains(_bossArenaBounds, pos, padding);

            if (isInfiniteMap)
                return true;

            if (mapHalfWidth <= 0f || mapHalfHeight <= 0f)
                return true;

            if ((-(mapHalfWidth - padding) > pos.x) || ((mapHalfWidth - padding) < pos.x) ||
                (-(mapHalfHeight - padding) > pos.y) || ((mapHalfHeight - padding) < pos.y))
            {
                return false;
            }
            return true;
        }

        public Vector2 ClampToMapBound(Vector2 pos, float padding)
        {
            if (_isBossArenaActive)
                return ClampToRect(_bossArenaBounds, pos, padding);

            if (isInfiniteMap || mapHalfWidth <= 0f || mapHalfHeight <= 0f)
                return pos;

            float maxX = Mathf.Max(0f, mapHalfWidth - padding);
            float maxY = Mathf.Max(0f, mapHalfHeight - padding);

            return new Vector2(
                Mathf.Clamp(pos.x, -maxX, maxX),
                Mathf.Clamp(pos.y, -maxY, maxY));
        }

        public void SetBossArenaBounds(Vector2 center, Vector2 size)
        {
            _bossArenaBounds = new Rect(center - size * 0.5f, size);
            _isBossArenaActive = true;
        }

        public void ClearBossArenaBounds()
        {
            _isBossArenaActive = false;
        }
        
        private void HandleMapSizeSet(MapSizeSetEvent evt)
        {
            mapHalfWidth = evt.Width / 2;
            mapHalfHeight = evt.Height / 2;
            isInfiniteMap = evt.IsInfinite;
        }

        private static bool Contains(Rect bounds, Vector2 pos, float padding)
        {
            float minX = Mathf.Min(bounds.xMin + padding, bounds.center.x);
            float maxX = Mathf.Max(bounds.xMax - padding, bounds.center.x);
            float minY = Mathf.Min(bounds.yMin + padding, bounds.center.y);
            float maxY = Mathf.Max(bounds.yMax - padding, bounds.center.y);

            return pos.x >= minX && pos.x <= maxX
                   && pos.y >= minY && pos.y <= maxY;
        }

        private static Vector2 ClampToRect(Rect bounds, Vector2 pos, float padding)
        {
            float minX = Mathf.Min(bounds.xMin + padding, bounds.center.x);
            float maxX = Mathf.Max(bounds.xMax - padding, bounds.center.x);
            float minY = Mathf.Min(bounds.yMin + padding, bounds.center.y);
            float maxY = Mathf.Max(bounds.yMax - padding, bounds.center.y);

            return new Vector2(
                Mathf.Clamp(pos.x, minX, maxX),
                Mathf.Clamp(pos.y, minY, maxY));
        }
    }
}
