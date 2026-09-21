using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaCameraTarget
    {
        private readonly float _cameraSizeTransitionDuration;
        private readonly BossArenaCameraTargetTween _transition;
        private CinemachineCamera _cinemachineCamera;
        private CameraTarget _previousCameraTarget;
        private Transform _arenaCameraTarget;
        private bool _hasPreviousCameraTarget;

        public BossArenaCameraTarget(float duration)
        {
            _cameraSizeTransitionDuration = duration;
            _transition = new BossArenaCameraTargetTween(duration);
        }

        public void Activate(CinemachineCamera camera, Vector2 arenaCenter)
        {
            _cinemachineCamera = camera;
            if (_cinemachineCamera == null)
                return;

            if (!_hasPreviousCameraTarget)
            {
                _previousCameraTarget = _cinemachineCamera.Target;
                _hasPreviousCameraTarget = true;
            }

            if (_arenaCameraTarget == null)
                _arenaCameraTarget = new GameObject("BossArenaCameraTarget").transform;

            Vector2 cameraCenter = GetCurrentCameraCenter();
            _arenaCameraTarget.position = new Vector3(cameraCenter.x, cameraCenter.y, 0f);

            CameraTarget target = _cinemachineCamera.Target;
            target.TrackingTarget = _arenaCameraTarget;
            target.LookAtTarget = null;
            target.CustomLookAtTarget = false;
            _cinemachineCamera.Target = target;

            _transition.Move(_arenaCameraTarget, arenaCenter);
        }

        public void Restore()
        {
            _transition.Cancel();

            Transform previousTrackingTarget = _previousCameraTarget.TrackingTarget;
            if (_cinemachineCamera == null
                || !_hasPreviousCameraTarget
                || _arenaCameraTarget == null
                || previousTrackingTarget == null
                || _cameraSizeTransitionDuration <= 0f)
            {
                CompleteCameraTargetRestore();
                return;
            }

            _transition.Restore(_arenaCameraTarget, previousTrackingTarget, CompleteCameraTargetRestore);
        }

        private void CompleteCameraTargetRestore()
        {
            if (_cinemachineCamera != null && _hasPreviousCameraTarget)
                _cinemachineCamera.Target = _previousCameraTarget;

            _hasPreviousCameraTarget = false;

            if (_arenaCameraTarget != null)
                Object.Destroy(_arenaCameraTarget.gameObject);

            _arenaCameraTarget = null;
        }

        private Vector2 GetCurrentCameraCenter()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                return mainCamera.transform.position;

            if (_cinemachineCamera != null)
                return _cinemachineCamera.transform.position;

            Transform previousTrackingTarget = _previousCameraTarget.TrackingTarget;
            return previousTrackingTarget != null
                ? (Vector2)previousTrackingTarget.position
                : Vector2.zero;
        }
    }
}
