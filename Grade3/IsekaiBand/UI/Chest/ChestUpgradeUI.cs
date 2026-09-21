using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Card;
using UnityEngine;

namespace _Work.CHUH.Code.UI.Chest
{
    // cnt는 1,3,5중 하나
    // cnt 수만큼 카드를 키고 데이터를 설정한다
    // 쿠킹덤식 연출로 오른쪽에서 하나씩 날아오는 연출 사용(오브젝트 켜두고 자식을 옮겼다가 다시 옮기면 됨)
    // 직렬화 필드에 5개 넣어놓고, 끄고키는 방식 사용(프리펩 생성하는 방식 X)
    public class ChestUpgradeUI : MonoBehaviour
    {
        public event Action OnOpenCompleted;
        public event Action OnCloseRequested;

        [SerializeField] private List<UpgradeCard> _upgrades; // 이거 부모가 따로 있음. 이거만 움직이면 자동으로 됨.
        [SerializeField] private CardSlotManagerCompo slotManagerCompo;

        private const float SlideOffset = 1200f;
        private const float MoveDuration = 0.35f;
        private const float RevealInterval = 0.12f;

        private readonly List<Vector2> _originPositions = new();
        private Coroutine _openRoutine;
        private bool _canClose;

        private void Awake()
        {
            CacheOriginPositions();
            SetCardsActive(0);
        }

        public void OpenChestUpgradeUI(int cnt)
        {
            _canClose = false;

            if (slotManagerCompo == null)
                slotManagerCompo = FindFirstObjectByType<CardSlotManagerCompo>();

            if (slotManagerCompo == null)
            {
                Debug.LogWarning("[ChestUpgradeUI] CardSlotManagerCompo를 찾을 수 없습니다.");
                SetCardsActive(0);
                OnOpenCompleted?.Invoke();
                return;
            }

            if (_originPositions.Count != _upgrades.Count)
                CacheOriginPositions();

            if (_openRoutine != null)
            {
                StopCoroutine(_openRoutine);
                _openRoutine = null;
            }

            _openRoutine = StartCoroutine(OpenRoutine(cnt));
        }

        public void Close()
        {
            if (_openRoutine != null)
            {
                StopCoroutine(_openRoutine);
                _openRoutine = null;
            }

            SetCardsActive(0);
            _canClose = false;
        }

        private System.Collections.IEnumerator OpenRoutine(int cnt)
        {
            IReadOnlyList<CardSlotManagerCompo.ChestUpgradeResult> results = slotManagerCompo.LastChestUpgradeResults;
            int showCount = Mathf.Min(Mathf.Max(1, cnt), _upgrades.Count, results.Count);

            if (showCount == 0)
            {
                SetCardsActive(0);
                _openRoutine = null;
                _canClose = true;
                OnOpenCompleted?.Invoke();
                yield break;
            }

            PrepareCardsForReveal(results, showCount);

            for (int i = 0; i < showCount; i++)
            {
                UpgradeCard view = _upgrades[i];
                RectTransform rect = view.transform as RectTransform;
                Vector2 target = _originPositions[i];
                Vector2 start = GetHiddenPosition(i);

                SetCardParentActive(i, true);
                yield return MoveCard(rect, start, target);

                if (i < showCount - 1)
                    yield return new WaitForSecondsRealtime(RevealInterval);
            }

            _openRoutine = null;
            _canClose = true;
            OnOpenCompleted?.Invoke();
        }

        private void HandleRewardClicked()
        {
            if (!_canClose) return;
            OnCloseRequested?.Invoke();
        }

        private void PrepareCardsForReveal(IReadOnlyList<CardSlotManagerCompo.ChestUpgradeResult> results, int showCount)
        {
            for (int i = 0; i < _upgrades.Count; i++)
            {
                bool active = i < showCount;
                SetCardParentActive(i, false);

                RectTransform rect = _upgrades[i].transform as RectTransform;
                if (rect == null || i >= _originPositions.Count) continue;

                if (!active)
                {
                    rect.anchoredPosition = _originPositions[i];
                    continue;
                }

                CardSlotManagerCompo.ChestUpgradeResult result = results[i];
                _upgrades[i].Setup(result.Card, result.Stack, false, HandleRewardClicked);
                rect.anchoredPosition = GetHiddenPosition(i);
            }
        }

        private System.Collections.IEnumerator MoveCard(RectTransform rect, Vector2 start, Vector2 target)
        {
            if (rect == null) yield break;

            float elapsed = 0f;
            while (elapsed < MoveDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / MoveDuration);
                t = 1f - Mathf.Pow(1f - t, 3f);
                rect.anchoredPosition = Vector2.LerpUnclamped(start, target, t);
                yield return null;
            }

            rect.anchoredPosition = target;
        }

        private Vector2 GetHiddenPosition(int index)
        {
            return _originPositions[index] + Vector2.right * SlideOffset;
        }

        private void CacheOriginPositions()
        {
            _originPositions.Clear();

            for (int i = 0; i < _upgrades.Count; i++)
            {
                RectTransform rect = _upgrades[i].transform as RectTransform;
                _originPositions.Add(rect != null ? rect.anchoredPosition : Vector2.zero);
            }
        }

        private void SetCardsActive(int activeCount)
        {
            for (int i = 0; i < _upgrades.Count; i++)
            {
                SetCardParentActive(i, i < activeCount);

                RectTransform rect = _upgrades[i].transform as RectTransform;
                if (rect != null && i < _originPositions.Count)
                    rect.anchoredPosition = _originPositions[i];
            }
        }

        private void SetCardParentActive(int index, bool active)
        {
            Transform parent = _upgrades[index].transform.parent;
            if (parent != null)
                parent.gameObject.SetActive(active);
            else
                _upgrades[index].gameObject.SetActive(active);
        }
    }
}
