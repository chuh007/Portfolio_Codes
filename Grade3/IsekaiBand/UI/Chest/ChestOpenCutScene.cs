using System;
using System.Collections;
using _Work.CHUH.Code.Core.Events;
using Chuh007Lib.Bus;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Work.CHUH.Code.UI.Chest
{
    public class ChestOpenCutScene : MonoBehaviour
    {
        [SerializeField] private Animator chest;
        public event Action OnChestUpgrade;
        
        private static int _openHash = Animator.StringToHash("OPEN");

        private const float DropDuration = 0.5f;
        private const float DropOffsetY = 700f;

        private RectTransform _rectTransform;
        private Vector2 _originAnchoredPosition;
        private bool _hasOriginPosition;
        private bool _isOpening;
        
        private void Awake()
        {
            chest.updateMode = AnimatorUpdateMode.UnscaledTime;
            _rectTransform = transform as RectTransform;
            CacheOriginPosition();
        }

        public IEnumerator PlayDropIn()
        {
            CacheOriginPosition();

            Vector2 target = _originAnchoredPosition;
            Vector2 start = target + Vector2.up * DropOffsetY;
            SetAnchoredPosition(start);

            float elapsed = 0f;
            while (elapsed < DropDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / DropDuration);
                t = 1f - Mathf.Pow(1f - t, 3f);
                SetAnchoredPosition(Vector2.LerpUnclamped(start, target, t));
                yield return null;
            }

            SetAnchoredPosition(target);
        }

        public void ResetDropInPosition()
        {
            CacheOriginPosition();
            SetAnchoredPosition(_originAnchoredPosition);
        }
        
        public void OpenChest()
        {
            _isOpening = true;
            chest.SetBool(_openHash, true);
        }

        public void ResetChest()
        {
            _isOpening = false;
            chest.SetBool(_openHash, false);
        }
        
        private void OnOpenEnd()
        {
            if (!_isOpening) return;

            _isOpening = false;
            OnChestUpgrade?.Invoke();
        }

        private void CacheOriginPosition()
        {
            if (_hasOriginPosition) return;

            if (_rectTransform == null)
                _rectTransform = transform as RectTransform;

            _originAnchoredPosition = _rectTransform != null ? _rectTransform.anchoredPosition : Vector2.zero;
            _hasOriginPosition = true;
        }

        private void SetAnchoredPosition(Vector2 position)
        {
            if (_rectTransform == null) return;
            _rectTransform.anchoredPosition = position;
        }
    }
}
