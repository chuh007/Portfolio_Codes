using System.Threading;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LaneSweepBlend
    {
        private readonly Phase2LaneSweepPatternSO _pattern;
        private CinemachineBlendDefinition _previousDefaultBlend;
        private bool _hasPreviousDefaultBlend;
        public LaneSweepBlend(Phase2LaneSweepPatternSO pattern) => _pattern = pattern;

        public void ApplyBrainBlendDuration(float duration)
        {
            if (_pattern.Camera.Brain == null)
                return;

            if (!_hasPreviousDefaultBlend)
            {
                _previousDefaultBlend = _pattern.Camera.Brain.DefaultBlend;
                _hasPreviousDefaultBlend = true;
            }

            _pattern.Camera.Brain.DefaultBlend = new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Styles.EaseInOut,
                Mathf.Max(0f, duration));
        }

        public void RestoreBrainBlendDuration()
        {
            if (_pattern.Camera.Brain != null && _hasPreviousDefaultBlend)
                _pattern.Camera.Brain.DefaultBlend = _previousDefaultBlend;

            _pattern.Camera.Brain = null;
            _hasPreviousDefaultBlend = false;
        }

        public static async UniTask WaitForCinemachineBlend(float duration, CancellationToken ct)
        {
            if (duration <= 0f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                return;
            }

            await UniTask.WaitForSeconds(duration, cancellationToken: ct).SuppressCancellationThrow();
        }
    }
}
