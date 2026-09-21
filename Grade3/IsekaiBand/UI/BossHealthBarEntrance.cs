using System.Threading;
using _Work.CHUH.Code.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.UI
{
    internal class BossHealthBarEntrance
    {
        private readonly MonoBehaviour _owner;
        private readonly BossHealthBarDisplay _display;
        private readonly Func<IHealth> _getHealth;
        private readonly Func<bool> _cacheElements;
        public bool IsPlaying { get; private set; }
        public bool IsHidden { get; private set; }

        public BossHealthBarEntrance(MonoBehaviour owner, BossHealthBarDisplay display,
            Func<IHealth> getHealth, Func<bool> cacheElements)
        {
            _owner = owner;
            _display = display;
            _getHealth = getHealth;
            _cacheElements = cacheElements;
        }

        public void HideUntilFillAnimation()
        {
            IsHidden = true;
            _cacheElements();

            if (_display.IsReady)
                _display.SetDisplayedRatio(0f, false);

            _display.SetRootVisible(false);
        }

        public async UniTask<bool> PlayFillAnimationAsync(
            float duration,
            CancellationToken cancellationToken)
        {
            if (_getHealth() == null)
                return false;

            for (int attempt = 0; attempt < 3 && !_cacheElements(); attempt++)
            {
                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken)
                    .SuppressCancellationThrow();
                if (canceled)
                    return true;
            }

            if (!_display.IsReady)
            {
                IsHidden = false;
                _display.SetRootVisible(true);
                return false;
            }

            float targetRatio = GetCurrentHealthRatio();
            float fillDuration = Mathf.Max(0f, duration);
            IsPlaying = true;

            try
            {
                _display.SetDisplayedRatio(0f, false);
                IsHidden = false;
                _display.SetRootVisible(true);

                if (fillDuration <= 0f)
                    return false;

                float elapsed = 0f;
                while (elapsed < fillDuration)
                {
                    float progress = Mathf.Clamp01(elapsed / fillDuration);
                    _display.SetDisplayedRatio(targetRatio * Mathf.SmoothStep(0f, 1f, progress), false);

                    bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken)
                        .SuppressCancellationThrow();
                    if (canceled)
                        return true;

                    elapsed += Time.unscaledDeltaTime;
                }

                _display.SetDisplayedRatio(targetRatio, false);
                return false;
            }
            finally
            {
                if (_owner != null)
                {
                    IsPlaying = false;
                    _display.SetDisplayedRatio(GetCurrentHealthRatio(), true);
                }
            }
        }

        private float GetCurrentHealthRatio()
        {
            return _getHealth() == null || _getHealth().MaxHealth <= 0f
                ? 0f
                : Mathf.Clamp01(_getHealth().CurrentHealth / _getHealth().MaxHealth);
        }

    }
}
