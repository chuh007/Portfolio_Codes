using _Work.CHUH.Code.EntityPlus.Effect;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    public sealed class StunEffectRuntime : MonoBehaviour
    {
        private Entity _entity;
        private IMover _mover;
        private EntityMover _entityMover;
        private Animator _animator;
        private bool _previousCanManualMove;
        private float _previousAnimatorSpeed = 1f;
        private int _activeStunCount;
        private bool _hasSnapshot;

        private void Awake()
        {
            enabled = false;
        }

        public void BeginStun()
        {
            if (_activeStunCount == 0)
                CaptureState();

            _activeStunCount++;
            enabled = true;
            ApplyStunState();
        }

        public void EndStun()
        {
            _activeStunCount = Mathf.Max(0, _activeStunCount - 1);
            if (_activeStunCount == 0)
                RestoreState();
        }

        private void Update()
        {
            if (_activeStunCount <= 0) return;

            if (_entity != null && _entity.IsDead)
            {
                CancelForDeath();
                return;
            }

            ApplyStunState();
        }

        private void OnDisable()
        {
            if (_activeStunCount <= 0) return;

            _activeStunCount = 0;
            RestoreState();
        }

        private void CaptureState()
        {
            _entity = GetComponent<Entity>();
            _mover = GetComponentInChildren<IMover>();
            _entityMover = _mover as EntityMover;
            _animator = GetComponentInChildren<Animator>();

            _previousCanManualMove = _mover == null || _mover.CanManualMove;
            _previousAnimatorSpeed = _animator != null ? _animator.speed : 1f;
            _hasSnapshot = true;
        }

        private void ApplyStunState()
        {
            if (_mover != null)
            {
                if (_entityMover != null)
                    _entityMover.SetMovementBlocked(this, true);
                else
                    _mover.CanManualMove = false;

                _mover.StopImmediately();
            }

            if (_animator != null)
                _animator.speed = 0f;
        }

        private void RestoreState()
        {
            if (!_hasSnapshot) return;

            if (_entityMover != null)
            {
                _entityMover.SetMovementBlocked(this, false);
                if (_entity != null && _entity.IsDead)
                    _entityMover.CanManualMove = false;
            }
            else if (_mover != null)
            {
                _mover.CanManualMove = _entity == null || !_entity.IsDead
                    ? _previousCanManualMove
                    : false;
            }

            if (_animator != null)
                _animator.speed = _previousAnimatorSpeed;

            _hasSnapshot = false;
            enabled = false;
        }

        private void CancelForDeath()
        {
            _activeStunCount = 0;

            if (_mover != null)
            {
                _entityMover?.SetMovementBlocked(this, false);
                _mover.CanManualMove = false;
                _mover.StopImmediately();
            }

            // 스턴은 애니메이터만 다시 재생시키고, 사망한 엔티티의 이동 잠금은 유지한다.
            // 그래야 Dead 상태의 애니메이션 이벤트가 끝까지 실행되어 풀 반환까지 진행된다.
            if (_animator != null)
                _animator.speed = _previousAnimatorSpeed;

            _hasSnapshot = false;
            enabled = false;
        }
    }
}
