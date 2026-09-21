using System.Threading;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class PianoHumanAppearance
    {
        private EntityRenderer _renderer;
        private Vector3 _rendererBaseLocalScale;
        private bool _hasRendererBaseLocalScale;

        public void Initialize(EntityRenderer renderer)
        {
            _renderer = renderer;
            if (_renderer != null && !_hasRendererBaseLocalScale)
            {
                _rendererBaseLocalScale = _renderer.transform.localScale;
                _hasRendererBaseLocalScale = true;
            }
        }

        public void Restore()
        {
            RestoreRendererScale();
            _renderer?.SetAlpha(1f);
        }

        public async UniTask<bool> PlayVanishAsync(
            PianoBossHumanFormTransition settings, CancellationToken cancellationToken)
        {
            if (_renderer == null)
                return false;

            Transform visual = _renderer.transform;
            Vector3 startScale = _hasRendererBaseLocalScale
                ? _rendererBaseLocalScale
                : visual.localScale;
            Vector3 targetScale = new Vector3(
                startScale.x,
                startScale.y * Mathf.Max(1f, settings.PhaseOneVanishVerticalScaleMultiplier),
                startScale.z);

            if (settings.PhaseOneVanishDuration <= 0f)
            {
                visual.localScale = targetScale;
                _renderer.SetAlpha(0f);
                return false;
            }

            float elapsed = 0f;
            while (elapsed < settings.PhaseOneVanishDuration)
            {
                float progress = Mathf.Clamp01(elapsed / settings.PhaseOneVanishDuration);
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                visual.localScale = Vector3.LerpUnclamped(startScale, targetScale, easedProgress);
                _renderer.SetAlpha(1f - easedProgress);

                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken)
                    .SuppressCancellationThrow();
                if (canceled)
                    return true;

                elapsed += Time.unscaledDeltaTime;
            }

            visual.localScale = targetScale;
            _renderer.SetAlpha(0f);
            return false;
        }

        private void RestoreRendererScale()
        {
            if (_renderer != null && _hasRendererBaseLocalScale)
                _renderer.transform.localScale = _rendererBaseLocalScale;
        }
    }
}
