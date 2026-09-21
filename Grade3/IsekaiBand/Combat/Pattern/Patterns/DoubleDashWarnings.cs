using System.Threading;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class DoubleDashWarnings
    {
        private readonly DoubleDashPatternSO _pattern;
        private const float MinAnimationDuration = 0.01f;
        public DoubleDashWarnings(DoubleDashPatternSO pattern) => _pattern = pattern;

        public async UniTask PlayWarning(Enemy owner, Vector2 direction, float durationSeconds, CancellationToken ct)
        {
            float warnLength = _pattern.Speed * _pattern.Duration;
            Vector2 warnPos = (Vector2)owner.transform.position + direction * (warnLength * 0.5f);
            float rotation = Vector2.SignedAngle(Vector2.up, direction);

            var warning = _pattern.poolManager.Pop(_pattern.warningItem) as RectWarning;
            warning.Setup(warnPos, rotation, _pattern.WarningWidth, warnLength, true);

            if (!_pattern.UseWarningAnimation)
            {
                await warning.PlayAsync(durationSeconds, ct);
                return;
            }

            PlayWarningAnimation(owner, durationSeconds);
            try
            {
                await warning.PlayAsync(durationSeconds, ct);
            }
            finally
            {
                _pattern.Animation.ResetAttackAnimation(owner);
            }
        }

        public void PlayWarningAnimation(Enemy owner, float durationSeconds)
        {
            Animator animator = PatternAnimatorUtility.GetAnimator(owner);
            if (!PatternAnimatorUtility.TryGetAnimatorState(animator, _pattern.WarningStateName, out int layerIndex, out int stateHash, _pattern.AnimatorLayer) &&
                !PatternAnimatorUtility.TryGetAnimatorState(animator, _pattern.LegacyWarningStateName, out layerIndex, out stateHash, _pattern.AnimatorLayer))
                return;

            PatternAnimatorUtility.SetAnimatorBoolIfExists(animator, _pattern.AttackBoolParameter, true);
            PatternAnimatorUtility.SetAttackBlendValue(animator, _pattern.BlendTreeValue, _pattern.AttackBlendParameter, _pattern.FallbackBlendParameter);
            animator.speed = GetNormalizedWarningSpeed(durationSeconds);
            animator.Play(stateHash, layerIndex, 0f);
            animator.Update(0f);
        }

        public float GetNormalizedWarningSpeed(float durationSeconds)
        {
            return Mathf.Max(MinAnimationDuration, _pattern.AuthoredWarningDuration) /
                   Mathf.Max(MinAnimationDuration, durationSeconds);
        }
    }
}
