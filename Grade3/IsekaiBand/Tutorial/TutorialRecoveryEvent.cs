using System;
using System.Collections;
using _Code.LCH._02.Scripts.Core;
using _Work.CHUH.Code.Combat;
using _Work.CHUH.Code.EntityPlus;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    internal sealed class TutorialRecoveryEvent : IHealthDepletionHandler, IDisposable
    {
        private readonly MonoBehaviour _owner;
        private readonly TutorialDialogueView _dialogue;
        private readonly EntityHealth _health;
        private readonly IHealthDepletionHandler _previousHandler;
        private Coroutine _routine;
        private Action _restorePresentation;
        private bool _hasRecovered;
        private bool _disposed;
        public bool IsRecovering { get; private set; }

        public TutorialRecoveryEvent(MonoBehaviour owner, TutorialDialogueView dialogue, EntityHealth health)
        {
            _owner = owner;
            _dialogue = dialogue;
            _health = health;
            _previousHandler = health.HealthDepletionHandler;
            health.HealthDepletionHandler = this;
        }

        public bool TryHandleHealthDepleted()
        {
            if (_disposed || _health == null || _health.MaxHealth <= 0f) return false;
            if (IsRecovering)
            {
                _health.TakeHeal(1f);
                return true;
            }
            if (_hasRecovered)
            {
                _health.SetCurrentHealthRatio(1f);
                return true;
            }

            _hasRecovered = true;
            IsRecovering = true;
            _health.TakeHeal(1f);
            _restorePresentation = _dialogue.CapturePresentation();
            GameplayPauseService.Acquire(_owner);
            _routine = _owner.StartCoroutine(Recover());
            return true;
        }

        private IEnumerator Recover()
        {
            try
            {
                yield return Say(TutorialSpeaker.Piano, "괜찮아! 연습 중이니까 다시 해 보자.");
                yield return Say(TutorialSpeaker.Drum, "위험할 땐 대시로 피해 봐!");
                yield return Say(TutorialSpeaker.Vocal, "체력을 전부 채워 줄게. 다시 시작하자!");
            }
            finally
            {
                FinishRecovery();
            }
        }

        private IEnumerator Say(TutorialSpeaker speaker, string message)
        {
            _dialogue.ShowDialogue(speaker, message);
            yield return new WaitUntil(() => _dialogue.AdvanceRequested);
        }

        private void FinishRecovery()
        {
            if (!IsRecovering) return;
            if (_health != null) _health.SetCurrentHealthRatio(1f);
            if (_dialogue != null)
            {
                if (_disposed) _dialogue.Hide();
                else _restorePresentation?.Invoke();
            }
            _restorePresentation = null;
            _routine = null;
            IsRecovering = false;
            GameplayPauseService.Release(_owner);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_routine != null) _owner.StopCoroutine(_routine);
            FinishRecovery();
            if (_health != null && ReferenceEquals(_health.HealthDepletionHandler, this))
                _health.HealthDepletionHandler = _previousHandler;
        }
    }
}
