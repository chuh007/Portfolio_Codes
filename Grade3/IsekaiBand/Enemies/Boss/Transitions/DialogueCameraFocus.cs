using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class DialogueCameraFocus : IDisposable
    {
        private readonly float _transitionDuration;
        private readonly DialogueFocusCamera _camera;
        private bool _isActive;

        public DialogueCameraFocus(float focusOrthographicSize, float transitionDuration)
        {
            _transitionDuration = Mathf.Max(0f, transitionDuration);
            _camera = new DialogueFocusCamera(focusOrthographicSize, transitionDuration);
        }

        public async UniTask<bool> FocusAsync(
            Transform target,
            CancellationToken cancellationToken)
        {
            if (target == null || !_camera.EnsureCamera())
                return false;

            Vector3 targetPosition = GetCameraPosition(target.position);
            if (!_isActive)
            {
                _camera.Camera.transform.position = targetPosition;
                _camera.Camera.Priority = (int)_camera.Source.Priority + 10;
                _isActive = true;
                return await WaitForTransition(cancellationToken);
            }

            if ((_camera.Camera.transform.position - targetPosition).sqrMagnitude <= 0.0001f)
                return false;

            return await MoveCameraAsync(targetPosition, cancellationToken);
        }

        public async UniTask RestoreAsync()
        {
            if (_camera.Camera == null)
                return;

            _camera.Camera.Priority = (int)(_camera.Source != null ? _camera.Source.Priority : 0) - 10;
            await WaitForTransition(CancellationToken.None);
            Dispose();
        }

        private async UniTask<bool> MoveCameraAsync(
            Vector3 targetPosition,
            CancellationToken cancellationToken)
        {
            Vector3 startPosition = _camera.Camera.transform.position;
            if (_transitionDuration <= 0f)
            {
                _camera.Camera.transform.position = targetPosition;
                return false;
            }

            float elapsed = 0f;
            while (elapsed < _transitionDuration)
            {
                bool canceled = await UniTask.Yield(
                        PlayerLoopTiming.Update,
                        cancellationToken)
                    .SuppressCancellationThrow();
                if (canceled || _camera.Camera == null)
                    return true;

                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.SmoothStep(
                    0f,
                    1f,
                    Mathf.Clamp01(elapsed / _transitionDuration));
                _camera.Camera.transform.position = Vector3.LerpUnclamped(
                    startPosition,
                    targetPosition,
                    progress);
            }

            _camera.Camera.transform.position = targetPosition;
            return false;
        }

        private async UniTask<bool> WaitForTransition(CancellationToken cancellationToken)
        {
            if (_transitionDuration <= 0f)
            {
                return await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken)
                    .SuppressCancellationThrow();
            }

            return await UniTask.WaitForSeconds(
                    _transitionDuration,
                    ignoreTimeScale: true,
                    cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
        }

        private Vector3 GetCameraPosition(Vector3 targetPosition)
        {
            float z = _camera.Camera != null
                ? _camera.Camera.transform.position.z
                : targetPosition.z;
            return new Vector3(targetPosition.x, targetPosition.y, z);
        }

        public void Dispose()
        {
            _camera.Dispose();
            _isActive = false;
        }
    }
}
