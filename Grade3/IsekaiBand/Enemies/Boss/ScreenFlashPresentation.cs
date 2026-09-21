using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Work.CHUH.Code.Enemies.Boss
{
    [Serializable]
    public sealed class ScreenFlashPresentation
    {
        [SerializeField] private Color color = Color.white;
        [SerializeField, Range(0f, 1f)] private float peakAlpha = 1f;
        [SerializeField, Min(0f)] private float flashInDuration = 0.07f;
        [SerializeField, Min(0f)] private float peakHoldDuration = 0.04f;
        [SerializeField, Min(0f)] private float flashOutDuration = 0.22f;
        [SerializeField] private int sortingOrder = 32000;

        public async UniTask<Handle> ShowAsync(CancellationToken cancellationToken)
        {
            Handle handle = CreateOverlay();
            bool canceled = await SetAlphaAsync(
                handle.CanvasGroup,
                0f,
                Mathf.Clamp01(peakAlpha),
                flashInDuration,
                cancellationToken);
            if (canceled)
            {
                Destroy(handle);
                return null;
            }

            if (peakHoldDuration > 0f)
            {
                canceled = await UniTask.Delay(
                        TimeSpan.FromSeconds(peakHoldDuration),
                        DelayType.UnscaledDeltaTime,
                        PlayerLoopTiming.Update,
                        cancellationToken)
                    .SuppressCancellationThrow();
                if (canceled)
                {
                    Destroy(handle);
                    return null;
                }
            }

            return handle;
        }

        public async UniTask<bool> HideAsync(Handle handle, CancellationToken cancellationToken)
        {
            if (handle == null)
                return false;

            try
            {
                return await SetAlphaAsync(
                    handle.CanvasGroup,
                    handle.CanvasGroup != null ? handle.CanvasGroup.alpha : 0f,
                    0f,
                    flashOutDuration,
                    cancellationToken);
            }
            finally
            {
                Destroy(handle);
            }
        }

        public void Destroy(Handle handle)
        {
            if (handle?.Root != null)
                UnityEngine.Object.Destroy(handle.Root);
        }

        private Handle CreateOverlay()
        {
            var root = new GameObject(
                "Boss Phase Screen Flash",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasGroup));

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasGroup canvasGroup = root.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            var panel = new GameObject("Flash", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(root.transform, false);

            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = panel.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            return new Handle(root, canvasGroup);
        }

        private static async UniTask<bool> SetAlphaAsync(
            CanvasGroup canvasGroup,
            float from,
            float to,
            float duration,
            CancellationToken cancellationToken)
        {
            if (canvasGroup == null)
                return false;

            if (duration <= 0f)
            {
                canvasGroup.alpha = to;
                return false;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken)
                    .SuppressCancellationThrow();
                if (canceled)
                    return true;

                elapsed += Time.unscaledDeltaTime;
            }

            canvasGroup.alpha = to;
            return false;
        }

        public sealed class Handle
        {
            public readonly GameObject Root;
            public readonly CanvasGroup CanvasGroup;

            public Handle(GameObject root, CanvasGroup canvasGroup)
            {
                Root = root;
                CanvasGroup = canvasGroup;
            }
        }
    }
}
