using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Enemies.Boss
{
    internal sealed class BossDeathCompletion
    {
        private readonly Boss _boss;
        private readonly Action _finish;
        public bool CompletionStarted;
        private bool _defeatPresentationStarted;
        private bool _defeatPresentationCompleted;
        private bool _defeatPresentationCanceled;
        private IBossDeathCompletionPresentation _deathCompletionPresentation;
        private bool _completionPresentationStarted;
        private bool _completionPresentationCompleted;
        private bool _completionPresentationCanceled;

        public BossDeathCompletion(Boss boss, Action finish)
        {
            _boss = boss;
            _finish = finish;
        }

        public void Initialize()
        {
            CompletionStarted = false;
            _defeatPresentationStarted = false;
            _defeatPresentationCompleted = false;
            _defeatPresentationCanceled = false;
            _deathCompletionPresentation = ResolveDeathCompletionPresentation();
            _completionPresentationStarted = false;
            _completionPresentationCompleted = false;
            _completionPresentationCanceled = false;
        }

        public void OnDeathStarted()
        {
            StartFinalDefeatPresentation();
            _deathCompletionPresentation?.OnDeathStarted(_boss.destroyCancellationToken);
        }

        public void CompleteDelayedDeath()
        {
            if (CompletionStarted)
                return;

            CompletionStarted = true;

            if (_boss.FinalDefeatPresentation == null && _deathCompletionPresentation == null)
            {
                _finish();
                return;
            }

            StartFinalDefeatPresentation();
            StartDeathCompletionPresentation();
            TryFinishFinalBossDeath();
        }

        private void StartFinalDefeatPresentation()
        {
            if (_boss.FinalDefeatPresentation == null || _defeatPresentationStarted)
                return;

            _defeatPresentationStarted = true;
            PlayFinalDefeatPresentationAsync().Forget();
        }

        private async UniTaskVoid PlayFinalDefeatPresentationAsync()
        {
            bool canceled = await _boss.FinalDefeatPresentation.PlayAsync(_boss.transform.position, _boss.destroyCancellationToken);
            if (_boss == null)
                return;

            _defeatPresentationCanceled = canceled;
            _defeatPresentationCompleted = true;
            TryFinishFinalBossDeath();
        }

        private void StartDeathCompletionPresentation()
        {
            if (_deathCompletionPresentation == null || _completionPresentationStarted)
                return;

            _completionPresentationStarted = true;
            PlayDeathCompletionPresentationAsync().Forget();
        }

        private async UniTaskVoid PlayDeathCompletionPresentationAsync()
        {
            bool canceled = await _deathCompletionPresentation.PlayBeforeCompletionAsync(
                _boss.destroyCancellationToken);
            if (_boss == null)
                return;

            _completionPresentationCanceled = canceled;
            _completionPresentationCompleted = true;
            TryFinishFinalBossDeath();
        }

        private void TryFinishFinalBossDeath()
        {
            if (!CompletionStarted
                || (_boss.FinalDefeatPresentation != null
                    && (!_defeatPresentationCompleted || _defeatPresentationCanceled))
                || (_deathCompletionPresentation != null
                    && (!_completionPresentationCompleted || _completionPresentationCanceled)))
                return;

            _finish();
        }

        private IBossDeathCompletionPresentation ResolveDeathCompletionPresentation()
        {
            MonoBehaviour[] behaviours = _boss.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IBossDeathCompletionPresentation presentation)
                    return presentation;
            }

            return null;
        }
    }
}
