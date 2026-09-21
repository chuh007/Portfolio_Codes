using System;
using System.Collections.Generic;
using System.Threading;
using _Work.CHUH.Code.Combat.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Work.CHUH.Code.Enemies.AttackCompo
{
    /// <summary>
    /// 중간보스 공격 관리. 패턴은 무작위 사용
    /// </summary>
    public class MiddleBossAttackCompo : EnemyAttackCompo
    {
        private static readonly int PatternAnimatorParameter = Animator.StringToHash("PATTERN");

        [SerializeField] private List<BasePatternSO> patterns;

        private int _lastPatternIndex = -1;
        private readonly List<int> _patternBag = new();
        private CancellationTokenSource _patternCts;
        
        public override void Attack(Action onAttackEnd)
        {
            base.Attack(onAttackEnd);

            CancelPattern();
            _patternCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            UsePatternAsync(onAttackEnd, _enemy.PoolLifecycleVersion, _patternCts).Forget();
        }

        protected override void OnDisable()
        {
            CancelPattern();
            base.OnDisable();
        }

        public void AddPattern(BasePatternSO pattern)
        {
            if (pattern == null)
                return;

            patterns ??= new List<BasePatternSO>();
            if (!patterns.Contains(pattern))
            {
                patterns.Add(pattern);
                _patternBag.Clear();
            }
        }

        private async UniTaskVoid UsePatternAsync(
            Action onAttackEnd,
            int lifecycleVersion,
            CancellationTokenSource cts)
        {
            bool shouldNotifyEnd = false;
            BasePatternSO selectedPattern = null;

            try
            {
                if (patterns == null || patterns.Count == 0)
                {
                    shouldNotifyEnd = true;
                    return;
                }

                int idx = SelectPatternIndex();
                if (idx < 0)
                {
                    Debug.LogError(
                        $"[{nameof(MiddleBossAttackCompo)}] No valid pattern reference is configured.",
                        this);
                    shouldNotifyEnd = true;
                    return;
                }

                _lastPatternIndex = idx;
                selectedPattern = patterns[idx];
                SetPatternPresentation(selectedPattern, true);
                await selectedPattern.UsePattern(_enemy, cts.Token);
                shouldNotifyEnd = true;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogError($"[{nameof(MiddleBossAttackCompo)}] Pattern attack failed.", this);
                Debug.LogException(exception, this);
                shouldNotifyEnd = true;
            }
            finally
            {
                SetPatternPresentation(selectedPattern, false);

                if (_patternCts == cts)
                    _patternCts = null;

                if (shouldNotifyEnd && CanNotifyAttackEnd(lifecycleVersion, cts))
                    onAttackEnd?.Invoke();

                cts.Dispose();
            }
        }

        private void SetPatternPresentation(BasePatternSO pattern, bool active)
        {
            if (pattern == null || !pattern.UsesPatternPresentationAnimation || _enemy == null)
                return;

            Animator animator = _enemy.GetComponentInChildren<Animator>();
            if (animator == null)
                return;

            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.nameHash != PatternAnimatorParameter
                    || parameter.type != AnimatorControllerParameterType.Bool)
                    continue;

                animator.SetBool(PatternAnimatorParameter, active);
                return;
            }
        }

        private bool CanNotifyAttackEnd(int lifecycleVersion, CancellationTokenSource cts)
        {
            return this != null
                   && isActiveAndEnabled
                   && !cts.IsCancellationRequested
                   && _enemy != null
                   && !_enemy.IsDead
                   && _enemy.IsCurrentPoolLifecycle(lifecycleVersion);
        }

        private void CancelPattern()
        {
            if (_patternCts == null) return;

            CancellationTokenSource cts = _patternCts;
            _patternCts = null;
            cts.Cancel();
        }

        private int SelectPatternIndex()
        {
            if (patterns == null || patterns.Count == 0)
                return -1;

            while (true)
            {
                while (_patternBag.Count > 0)
                {
                    int bagIndex = _patternBag.Count - 1;
                    int patternIndex = _patternBag[bagIndex];
                    _patternBag.RemoveAt(bagIndex);

                    if (patternIndex >= 0
                        && patternIndex < patterns.Count
                        && patterns[patternIndex] != null)
                        return patternIndex;
                }

                FillPatternBag();
                if (_patternBag.Count == 0)
                    return -1;
            }
        }

        private void FillPatternBag()
        {
            _patternBag.Clear();
            for (int i = 0; i < patterns.Count; i++)
            {
                if (patterns[i] != null)
                    _patternBag.Add(i);
            }

            for (int i = _patternBag.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Range(0, i + 1);
                (_patternBag[i], _patternBag[swapIndex]) = (_patternBag[swapIndex], _patternBag[i]);
            }

            int nextPatternPosition = _patternBag.Count - 1;
            if (_patternBag.Count <= 1
                || _patternBag[nextPatternPosition] != _lastPatternIndex)
                return;

            for (int i = 0; i < nextPatternPosition; i++)
            {
                if (_patternBag[i] == _lastPatternIndex)
                    continue;

                (_patternBag[i], _patternBag[nextPatternPosition]) =
                    (_patternBag[nextPatternPosition], _patternBag[i]);
                break;
            }
        }
    }
}
