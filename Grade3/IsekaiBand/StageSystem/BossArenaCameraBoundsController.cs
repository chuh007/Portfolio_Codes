using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaCameraBoundsController
    {
        private readonly float _defaultCameraOrthographicSize;
        private readonly BossArenaCameraTarget _target;
        private readonly BossArenaCameraLens _lens;
        private float _targetCameraOrthographicSize;
        private CinemachineCamera _cinemachineCamera;

        public BossArenaCameraBoundsController(
            Vector2 arenaSize, float targetCameraOrthographicSize, float cameraSizeTransitionDuration)
        {
            _defaultCameraOrthographicSize = Mathf.Max(1f, targetCameraOrthographicSize);
            _targetCameraOrthographicSize = _defaultCameraOrthographicSize;
            float duration = Mathf.Max(0f, cameraSizeTransitionDuration);
            _target = new BossArenaCameraTarget(duration);
            _lens = new BossArenaCameraLens(Vector2.Max(Vector2.one, arenaSize), duration);
        }

        public void Activate(Vector2 arenaCenter)
        {
            if (_cinemachineCamera == null)
                _cinemachineCamera = Object.FindAnyObjectByType<CinemachineCamera>();
            if (_cinemachineCamera == null) return;

            _target.Activate(_cinemachineCamera, arenaCenter);
            _lens.Apply(_cinemachineCamera, _targetCameraOrthographicSize);
        }

        public void Deactivate()
        {
            _target.Restore();
            _lens.Restore();
            _targetCameraOrthographicSize = _defaultCameraOrthographicSize;
        }

        public void SetTargetOrthographicSize(float orthographicSize)
        {
            _targetCameraOrthographicSize = Mathf.Max(1f, orthographicSize);
            if (_cinemachineCamera != null && _lens.HasPreviousLens)
                _lens.Apply(_cinemachineCamera, _targetCameraOrthographicSize);
        }

        internal static float SmootherStep(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }
    }
}
