using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaCameraLens
    {
        private const float ArenaFitVerticalPadding = 0.25f;
        private readonly Vector2 _arenaSize;
        private readonly BossArenaPixelPerfectOverride _pixelPerfect = new();
        private readonly BossArenaCameraLensTween _transition;
        private CinemachineCamera _cinemachineCamera;
        private LensSettings _previousCameraLens;
        private bool _hasPreviousCameraLens;
        public bool HasPreviousLens => _hasPreviousCameraLens;

        public BossArenaCameraLens(Vector2 arenaSize, float duration)
        {
            _arenaSize = arenaSize;
            _transition = new BossArenaCameraLensTween(duration, _pixelPerfect);
        }

        public void Apply(CinemachineCamera camera, float targetOrthographicSize)
        {
            _cinemachineCamera = camera;
            if (_cinemachineCamera == null)
                return;

            if (!_hasPreviousCameraLens)
            {
                _previousCameraLens = _cinemachineCamera.Lens;
                _hasPreviousCameraLens = true;
            }

            LensSettings targetLens = _cinemachineCamera.Lens;
            targetLens.OrthographicSize = Mathf.Max(targetOrthographicSize, CalculateArenaFitSize());
            targetLens.ModeOverride = LensSettings.OverrideModes.Orthographic;

            _transition.Start(_cinemachineCamera, targetLens);
        }

        public void Restore()
        {
            if (_cinemachineCamera == null || !_hasPreviousCameraLens)
            {
                _pixelPerfect.Restore();
                return;
            }

            _transition.Start(_cinemachineCamera, _previousCameraLens);
            _hasPreviousCameraLens = false;
        }

        private float CalculateArenaFitSize()
        {
            float aspect = GetCameraAspect();
            float halfHeight = _arenaSize.y * 0.5f;
            float halfWidthAsHeight = _arenaSize.x / (2f * Mathf.Max(0.01f, aspect));
            return Mathf.Max(1f, halfHeight, halfWidthAsHeight) + ArenaFitVerticalPadding;
        }

        private static float GetCameraAspect()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null && mainCamera.aspect > 0f)
                return mainCamera.aspect;

            return Screen.height > 0 ? Screen.width / (float)Screen.height : 16f / 9f;
        }
    }
}
