using System;
using System.Threading;
using _Work.CHUH.Code.Combat.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies
{
    /// <summary>
    /// 공격 없이 몸통 박치기만 하는 에너미
    /// </summary>
    public class BodySlamEnemy : FSMEnemy
    {
        [Header("Pattern")]
        [SerializeField] private BasePatternSO singlePattern;
        [SerializeField, Min(0f)] private float patternTriggerRange = 3f;

        private CancellationTokenSource _patternCts;
        private bool _isPatternRunning;

        protected override void Awake()
        {
            base.Awake();
        }

        protected void Start()
        {
            _stateMachine.ChangeState(StateName.Move);
        }

        protected override void Update()
        {
            base.Update();
            TryStartSinglePattern();
        }

        private void OnDisable()
        {
            CancelSinglePattern();
        }

        protected override void HandleHit()
        {
        }

        protected override void HandleDead()
        {
            CancelSinglePattern();
            base.HandleDead();
        }

        private void TryStartSinglePattern()
        {
            if (singlePattern == null || _isPatternRunning || IsDead || target == null)
                return;

            if (patternTriggerRange > 0f)
            {
                Vector2 offset = target.transform.position - transform.position;
                if (offset.sqrMagnitude > patternTriggerRange * patternTriggerRange)
                    return;
            }

            CancelSinglePattern();
            _patternCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            _isPatternRunning = true;
            RunSinglePatternAsync(singlePattern, _patternCts).Forget();
        }

        private async UniTaskVoid RunSinglePatternAsync(BasePatternSO pattern, CancellationTokenSource cts)
        {
            try
            {
                await pattern.UsePattern(this, cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogError($"[{nameof(BodySlamEnemy)}] Pattern failed. pattern={pattern.name}", this);
                Debug.LogException(exception, this);
            }
            finally
            {
                if (_patternCts == cts)
                {
                    _patternCts = null;
                    _isPatternRunning = false;
                }

                cts.Dispose();
            }
        }

        private void CancelSinglePattern()
        {
            if (_patternCts == null) return;

            _patternCts.Cancel();
            _patternCts = null;
            _isPatternRunning = false;
        }
    }
}
