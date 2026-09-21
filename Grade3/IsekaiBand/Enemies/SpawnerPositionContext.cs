using _Code.LCH._02.Scripts.Player;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    internal readonly struct SpawnerPositionContext
    {
        public readonly Player Player;
        public readonly PlayerMovementCompo PlayerMovement;
        public readonly Camera MainCamera;
        public readonly float MinDistance;
        public readonly float MaxDistance;
        public readonly float ForwardRespawnAngle;
        public readonly float SpawnViewportMargin;
        public readonly float DespawnViewportMargin;
        public readonly float MapHalfWidth;
        public readonly float MapHalfHeight;
        public readonly bool IsInfiniteMap;
        public readonly bool HasCameraSize;
        public readonly Vector2 SpawnMinHalfSize;
        public readonly Vector2 SpawnMaxHalfSize;
        public readonly Vector2 DespawnHalfSize;

        public SpawnerPositionContext(
            Player player,
            PlayerMovementCompo playerMovement,
            Camera mainCamera,
            float minDistance,
            float maxDistance,
            float forwardRespawnAngle,
            float spawnViewportMargin,
            float despawnViewportMargin,
            float mapHalfWidth,
            float mapHalfHeight,
            bool isInfiniteMap,
            bool hasCameraSize,
            Vector2 spawnMinHalfSize,
            Vector2 spawnMaxHalfSize,
            Vector2 despawnHalfSize)
        {
            Player = player;
            PlayerMovement = playerMovement;
            MainCamera = mainCamera;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            ForwardRespawnAngle = forwardRespawnAngle;
            SpawnViewportMargin = spawnViewportMargin;
            DespawnViewportMargin = despawnViewportMargin;
            MapHalfWidth = mapHalfWidth;
            MapHalfHeight = mapHalfHeight;
            IsInfiniteMap = isInfiniteMap;
            HasCameraSize = hasCameraSize;
            SpawnMinHalfSize = spawnMinHalfSize;
            SpawnMaxHalfSize = spawnMaxHalfSize;
            DespawnHalfSize = despawnHalfSize;
        }

        public bool HasMapBounds => MapHalfWidth > 0f && MapHalfHeight > 0f;

        public Vector2 PlayerPosition => Player != null ? (Vector2)Player.transform.position : Vector2.zero;

        public Vector2 CameraCenter => MainCamera != null ? (Vector2)MainCamera.transform.position : PlayerPosition;
    }
}
