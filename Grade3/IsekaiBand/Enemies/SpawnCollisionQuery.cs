using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Enemies
{
    internal class SpawnCollisionQuery
    {
        private const int MaxSpawnPositionAttempts = 10;
        private const float SpawnClearanceRadius = 0.5f;
        private TilemapCollider2D[] _blockingTilemapColliders;
        private int _wallLayerMask;

        public void Initialize()
        {
            int wallLayer = LayerMask.NameToLayer("Wall");
            _wallLayerMask = wallLayer >= 0 ? 1 << wallLayer : 0;
        }

        public bool TryGetUnblockedSpawnPosition(SpawnerPositionContext positionContext, Vector2 initialPosition,
            out Vector2 spawnPosition, bool requireOffscreen = false)
        {
            spawnPosition = default;
            if (requireOffscreen && !positionContext.HasCameraSize) return false;

            for (int attempt = 0; attempt < MaxSpawnPositionAttempts; attempt++)
            {
                Vector2 candidate = attempt == 0
                    ? initialPosition
                    : SpawnerPositionUtility.RandomSpawnPosition(positionContext);
                candidate = SpawnerPositionUtility.ClampToMapBounds(positionContext, candidate, 0f);
                if (requireOffscreen && !SpawnCameraBounds.IsOutsideSpawnBounds(positionContext, candidate))
                    continue;

                if (!HasBlockingEnvironment(candidate))
                {
                    spawnPosition = candidate;
                    return true;
                }
            }

            return false;
        }

        private bool HasBlockingEnvironment(Vector2 position)
        {
            CacheBlockingTilemapColliders();

            foreach (TilemapCollider2D tilemapCollider in _blockingTilemapColliders)
            {
                if (tilemapCollider == null || !tilemapCollider.isActiveAndEnabled)
                    continue;

                Tilemap tilemap = tilemapCollider.GetComponent<Tilemap>();
                if (tilemap == null)
                    continue;

                Vector3Int cell = tilemap.WorldToCell(position);
                if (tilemap.GetColliderType(cell) != Tile.ColliderType.None)
                    return true;
            }

            return _wallLayerMask != 0
                   && Physics2D.OverlapCircle(position, SpawnClearanceRadius, _wallLayerMask) != null;
        }

        private void CacheBlockingTilemapColliders()
        {
            if (_blockingTilemapColliders != null && _blockingTilemapColliders.Length > 0)
                return;

            _blockingTilemapColliders = Object.FindObjectsByType<TilemapCollider2D>(FindObjectsSortMode.None);
        }
    }
}
