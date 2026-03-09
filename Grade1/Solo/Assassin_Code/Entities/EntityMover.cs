using System;
using Code.Core.StatSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Code.Entities
{
    public class EntityMover : MonoBehaviour, IEntityComponent, IAfterInit
    {
        public UnityEvent<Vector2> OnVelocity;
        public UnityEvent<float> OnXInput;

        [SerializeField] private StatSO moveSpeedStat, jumpPowerStat;

        [Header("Collision detection")] 
        [SerializeField] private Transform groundCheckTrm;
        [SerializeField] private Transform wallCheckTrm;
        [SerializeField] private float groundCheckDistance, groundBoxWidth, wallCheckDistance;
        [SerializeField] private LayerMask whatIsGround;
        
        #region Member field

        private float _movementX;
        private float _moveSpeed = 6f;
        private float _moveSpeedMultiplier;
        private float _jumpPower;
        private float _originalGravityScale;
        private float _limitYSpeed = 40f;

        private Rigidbody2D _rbCompo;
        private EntityStat _statCompo;
        private Vector2 _colliderOffset, _colliderSize;
        
        #endregion

        public bool CanManualMove { get; set; } = true; //넉백당하거나 기절시 이동불가
        public CapsuleCollider2D BodyCollider { get; private set; }
        
        #region Init section
        public void Initialize(Entity entity)
        {
            _rbCompo = entity.GetComponent<Rigidbody2D>();
            _statCompo = entity.GetCompo<EntityStat>();
            _moveSpeedMultiplier = 1f;
            _originalGravityScale = _rbCompo.gravityScale;
            BodyCollider = entity.GetComponent<CapsuleCollider2D>();
            _colliderOffset = BodyCollider.offset;
            _colliderSize = BodyCollider.size;
        }
        
        public void AfterInit()
        {
            _statCompo.GetStat(moveSpeedStat).OnValueChange += HandleMoveSpeedChange;
            _statCompo.GetStat(jumpPowerStat).OnValueChange += HandleJumpPowerChange;
            
            _moveSpeed = _statCompo.GetStat(moveSpeedStat).Value;
            _jumpPower = _statCompo.GetStat(jumpPowerStat).Value;
        }

        private void OnDestroy()
        {
            _statCompo.GetStat(moveSpeedStat).OnValueChange -= HandleMoveSpeedChange;
            _statCompo.GetStat(jumpPowerStat).OnValueChange -= HandleJumpPowerChange;
        }
        #endregion

        public void Jump() => AddForceToEntity(new Vector2(0, _jumpPower));
        
        public void AddForceToEntity(Vector2 force)
            => _rbCompo.AddForce(force, ForceMode2D.Impulse);

        
        public void SetMoveSpeedMultiplier(float value)
            => _moveSpeedMultiplier = value;
        public void SetGravityScale(float value) 
            => _rbCompo.gravityScale = _originalGravityScale * value;
        public void SetLimitYSpeed(float value)
            => _limitYSpeed = value;

        public void SetColliderSize(Vector2 size, Vector2 offset)
        {
            BodyCollider.size = size;
            BodyCollider.offset = offset;
        }

        public void ResetColliderSize()
        {
            BodyCollider.size = _colliderSize;
            BodyCollider.offset = _colliderOffset;
        }

        private void HandleMoveSpeedChange(StatSO stat, float current, float previous)
            => _moveSpeed = current;
        private void HandleJumpPowerChange(StatSO stat, float current, float previous)
            => _jumpPower = current;

        private void FixedUpdate()
        {
            if(CanManualMove)
                _rbCompo.linearVelocityX = _movementX * _moveSpeed * _moveSpeedMultiplier;
            
            _rbCompo.linearVelocityY = Mathf.Clamp(_rbCompo.linearVelocityY, -_limitYSpeed, _limitYSpeed);
            OnVelocity?.Invoke(_rbCompo.linearVelocity);
        }

        public void SetMovementX(float xMovement)
        {
            _movementX = Mathf.Abs(xMovement) > 0 ? Mathf.Sign(xMovement) : 0;
            OnXInput?.Invoke(_movementX);
        }

        public void StopImmediately(bool isYAxisToo)
        {
            if (isYAxisToo)
                _rbCompo.linearVelocity = Vector2.zero;
            else
                _rbCompo.linearVelocityX = 0;

            _movementX = 0;
        }

        public void KnockBack(Vector2 force, float time)
        {
            CanManualMove = false;
            StopImmediately(true);
            AddForceToEntity(force);
            DOVirtual.DelayedCall(time, () => CanManualMove = true);
        }

        #region Check Collision

        public bool IsGroundDetected()
        {
            float boxHeight = 0.05f;
            Vector2 boxSize = new Vector2(groundBoxWidth, boxHeight);
            return Physics2D.BoxCast(groundCheckTrm.position, boxSize, 0, Vector2.down, groundCheckDistance, whatIsGround);
        }

        public bool IsWallDetected(float facingDirection)
            => Physics2D.Raycast(wallCheckTrm.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        
        public bool CheckColliderInFront(Vector2 dashDirection, float maxDistance, out float distance)
        {
            Bounds colliderBound = BodyCollider.bounds;
            Vector2 center = colliderBound.center;
            Vector2 size = colliderBound.size;
            size.y -= 0.2f; //바닥에 닿지 않도록 사이즈를 줄여준다.

            RaycastHit2D hit = Physics2D.BoxCast(center, size, 0, dashDirection, maxDistance, whatIsGround);

            distance = hit ? hit.distance : maxDistance;
            return hit;
        }
        
        #endregion
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (groundCheckTrm != null)
            {
                Gizmos.DrawWireCube(groundCheckTrm.position - new Vector3(0, groundCheckDistance * 0.5f), 
                                new Vector3(groundBoxWidth, groundCheckDistance, 1f));
            }
            
            if(wallCheckTrm != null)
                Gizmos.DrawLine(wallCheckTrm.position, wallCheckTrm.position + new Vector3(wallCheckDistance, 0));
        }
#endif

        
    }
}