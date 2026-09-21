using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.StageSystem
{
    internal sealed class BossArenaCameraTargetTween
    {
        private readonly float _duration;
        private CancellationTokenSource _transitionCts;

        public BossArenaCameraTargetTween(float duration) => _duration = duration;
        public void Cancel() => _transitionCts?.Cancel();

        public void Move(Transform target, Vector2 destination)
            => Start(target, null, destination, null);

        public void Restore(Transform target, Transform trackingTarget, Action complete)
            => Start(target, trackingTarget, default, complete);

        private void Start(Transform target, Transform trackingTarget, Vector2 destination, Action complete)
        {
            Cancel();
            var cts = new CancellationTokenSource();
            _transitionCts = cts;
            TransitionAsync(target, trackingTarget, destination, complete, cts).Forget();
        }

        private async UniTaskVoid TransitionAsync(
            Transform target, Transform trackingTarget, Vector2 destination,
            Action complete, CancellationTokenSource cts)
        {
            CancellationToken ct = cts.Token;
            bool restoring = complete != null;
            try
            {
                if (target == null) return;

                Vector3 start = target.position;
                Vector3 end = new Vector3(destination.x, destination.y, start.z);
                if (_duration <= 0f)
                {
                    target.position = end;
                    return;
                }

                float elapsed = 0f;
                while (elapsed < _duration)
                {
                    if (ct.IsCancellationRequested) return;
                    if (target == null || (restoring && trackingTarget == null))
                    {
                        if (restoring) complete();
                        return;
                    }
                    if (restoring)
                    {
                        end = trackingTarget.position;
                        end.z = start.z;
                    }

                    elapsed += Time.deltaTime;
                    float eased = BossArenaCameraBoundsController.SmootherStep(Mathf.Clamp01(elapsed / _duration));
                    target.position = Vector3.LerpUnclamped(start, end, eased);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                }

                if (!ct.IsCancellationRequested)
                {
                    if (restoring) complete();
                    else if (target != null) target.position = end;
                }
            }
            finally
            {
                if (_transitionCts == cts)
                    _transitionCts = null;
                cts.Dispose();
            }
        }
    }
}
