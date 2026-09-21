using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaCameraLensTween
    {
        private readonly float _cameraSizeTransitionDuration;
        private readonly BossArenaPixelPerfectOverride _pixelPerfect;
        private CinemachineCamera _cinemachineCamera;
        private CancellationTokenSource _cameraSizeTransitionCts;

        public BossArenaCameraLensTween(float duration, BossArenaPixelPerfectOverride pixelPerfect)
        {
            _cameraSizeTransitionDuration = duration;
            _pixelPerfect = pixelPerfect;
        }

        public void Start(CinemachineCamera camera, LensSettings targetLens)
        {
            _cinemachineCamera = camera;
            _cameraSizeTransitionCts?.Cancel();
            _pixelPerfect.Suspend(camera);
            var cts = new CancellationTokenSource();
            _cameraSizeTransitionCts = cts;
            TransitionCameraLensAsync(targetLens, cts).Forget();
        }

        private async UniTaskVoid TransitionCameraLensAsync(
            LensSettings targetLens,
            CancellationTokenSource cts)
        {
            CancellationToken ct = cts.Token;
            bool completed = false;
            try
            {
                if (_cinemachineCamera == null)
                    return;

                LensSettings startLens = _cinemachineCamera.Lens;
                float duration = _cameraSizeTransitionDuration;
                if (duration <= 0f)
                {
                    SetCameraLens(targetLens);
                    completed = true;
                    return;
                }

                float elapsed = 0f;
                while (elapsed < duration)
                {
                    if (ct.IsCancellationRequested || _cinemachineCamera == null)
                        return;

                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    float eased = BossArenaCameraBoundsController.SmootherStep(t);

                    LensSettings lens = startLens;
                    lens.OrthographicSize = Mathf.Lerp(
                        startLens.OrthographicSize,
                        targetLens.OrthographicSize,
                        eased);
                    lens.ModeOverride = targetLens.ModeOverride;
                    SetCameraLens(lens);

                    await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                }

                if (!ct.IsCancellationRequested)
                {
                    SetCameraLens(targetLens);
                    completed = true;
                }
            }
            finally
            {
                if (completed)
                    _pixelPerfect.Restore();

                if (_cameraSizeTransitionCts == cts)
                    _cameraSizeTransitionCts = null;

                cts.Dispose();
            }
        }

        private void SetCameraLens(LensSettings lens)
        {
            if (_cinemachineCamera == null)
                return;

            _cinemachineCamera.Lens = lens;
        }
    }
}
