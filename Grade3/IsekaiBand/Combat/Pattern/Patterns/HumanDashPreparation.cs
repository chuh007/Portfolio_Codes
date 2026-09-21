using System.Threading;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class HumanDashPreparation
    {
        private readonly PianoBossHumanDashPatternSO _pattern;
        private static readonly int DashAnimatorParameter = Animator.StringToHash("DASH");
        public Vector2 Direction;
        public Animator PreparedAnimator;
        private EntityMover _preparedMover;
        private bool _previousCanManualMove;
        public HumanDashPreparation(PianoBossHumanDashPatternSO pattern) => _pattern = pattern;

        public async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            RestorePreparedState();

            if (!TryGetTargetDirection(owner, out _pattern.Preparation.Direction))
                return;

            EntityRenderer renderer = owner.GetCompo<EntityRenderer>();
            renderer?.FlipController(_pattern.Preparation.Direction.x);

            _pattern.Preparation.PreparedAnimator = owner.GetComponentInChildren<Animator>();
            _preparedMover = owner.GetCompo<EntityMover>();
            _previousCanManualMove = _preparedMover != null && _preparedMover.CanManualMove;
            _preparedMover?.StopImmediately();
            if (_preparedMover != null)
                _preparedMover.CanManualMove = false;

            try
            {
                await PlayWarning(owner, ct);
            }
            catch
            {
                RestorePreparedState();
                throw;
            }
        }

        public async UniTask PlayWarning(Enemy owner, CancellationToken ct)
        {
            if (_pattern.WarningDuration <= 0f)
                return;

            if (_pattern.poolManager == null || _pattern.warningItem == null)
            {
                await UniTask.WaitForSeconds(_pattern.WarningDuration, cancellationToken: ct);
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.warningItem);
            if (pooled is RectWarning warning)
            {
                float rotation = Vector2.SignedAngle(Vector2.up, _pattern.Preparation.Direction);
                Vector2 warningCenter = (Vector2)owner.transform.position
                                        + _pattern.Preparation.Direction * (_pattern.DashDistance * 0.5f);
                warning.Setup(
                    warningCenter,
                    rotation,
                    _pattern.WarningWidth,
                    _pattern.DashDistance,
                    true);
                await warning.PlayAsync(_pattern.WarningDuration, ct);
                return;
            }

            if (pooled != null)
                _pattern.poolManager.Push(pooled);
            await UniTask.WaitForSeconds(_pattern.WarningDuration, cancellationToken: ct);
        }

        public void RestorePreparedState()
        {
            if (_pattern.Preparation.PreparedAnimator != null)
                _pattern.Preparation.PreparedAnimator.SetBool(DashAnimatorParameter, false);

            if (_preparedMover != null)
                _preparedMover.CanManualMove = _previousCanManualMove;

            _pattern.Preparation.Direction = Vector2.zero;
            _pattern.Preparation.PreparedAnimator = null;
            _preparedMover = null;
        }

        public static bool TryGetTargetDirection(Enemy owner, out Vector2 direction)
        {
            direction = Vector2.zero;
            if (owner == null || owner.target == null)
                return false;

            direction = owner.target.transform.position - owner.transform.position;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
                return false;

            direction.Normalize();
            return true;
        }
    }
}
