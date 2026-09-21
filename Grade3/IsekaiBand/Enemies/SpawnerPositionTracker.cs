using System;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Core;
using _Work.CHUH.Code.Core.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Enemies
{
    internal class SpawnerPositionTracker
    {
        private const float SpawnBandThicknessRatio = 0.1f;
        private const float DespawnBandGapRatio = 0.1f;
        private readonly Func<SpawnerSettings> _getSettings;
        private Player _player;
        private PlayerMovementCompo _playerMovement;
        private Camera _mainCamera;
        private float _mapHalfWidth, _mapHalfHeight;
        private bool _isInfiniteMap, _hasCameraSize;
        private Vector2 _screenHalfSize, _spawnMinHalfSize, _spawnMaxHalfSize, _despawnHalfSize;
        public Player Player => _player;

        public SpawnerPositionTracker(Func<SpawnerSettings> getSettings) => _getSettings = getSettings;

        public void Initialize()
        {
            _player = Object.FindAnyObjectByType<Player>();
            if (_player != null) _playerMovement = _player.GetComponent<PlayerMovementCompo>();
            _mainCamera = Camera.main;
        }

        public void HandleMapSize(MapSizeSetEvent evt)
        {
            _mapHalfWidth = evt.Width / 2;
            _mapHalfHeight = evt.Height / 2;
            _isInfiniteMap = evt.IsInfinite;
        }

        public void ResolvePlayer()
        {
            if (_player != null)
            {
                if (_playerMovement == null)
                    _playerMovement = _player.GetComponent<PlayerMovementCompo>();
                return;
            }

            _player = Object.FindAnyObjectByType<Player>();
            if (_player != null)
                _playerMovement = _player.GetComponent<PlayerMovementCompo>();
        }

        public SpawnerPositionContext CreatePositionContext()
        {
            CacheCameraSize();
            SpawnerSettings settings = _getSettings();
            return new SpawnerPositionContext(
                _player,
                _playerMovement,
                _mainCamera,
                settings.minDistance,
                settings.maxDistance,
                settings.forwardRespawnAngle,
                settings.spawnViewportMargin,
                settings.despawnViewportMargin,
                _mapHalfWidth,
                _mapHalfHeight,
                _isInfiniteMap,
                _hasCameraSize,
                _spawnMinHalfSize,
                _spawnMaxHalfSize,
                _despawnHalfSize);
        }

        public void CacheCameraSize()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            if (_mainCamera == null || !_mainCamera.orthographic)
            {
                _hasCameraSize = false;
                return;
            }

            float halfHeight = _mainCamera.orthographicSize;
            float halfWidth = halfHeight * _mainCamera.aspect;
            _screenHalfSize = new Vector2(halfWidth, halfHeight);
            _hasCameraSize = halfWidth > 0f && halfHeight > 0f;
            if (!_hasCameraSize)
                return;

            SpawnerSettings settings = _getSettings();
            _spawnMinHalfSize = _screenHalfSize * (1f + settings.spawnViewportMargin);
            _spawnMaxHalfSize = _spawnMinHalfSize + _screenHalfSize * SpawnBandThicknessRatio;
            Vector2 desiredDespawnHalfSize = _screenHalfSize * (1f + settings.despawnViewportMargin);
            Vector2 minDespawnHalfSize = _spawnMaxHalfSize + _screenHalfSize * DespawnBandGapRatio;
            _despawnHalfSize = new Vector2(
                Mathf.Max(desiredDespawnHalfSize.x, minDespawnHalfSize.x),
                Mathf.Max(desiredDespawnHalfSize.y, minDespawnHalfSize.y));
        }

    }
}
