using _01Scripts.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _01Scripts.Players
{
    public class CharacterMovement : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform parent;
        
        private float _moveSpeed = 3f;
        private float _runMoveSpeedMultiply = 3f;
        private bool _isRunning = false;

        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }
        
        public bool IsGround => controller.isGrounded;
        public bool CanManualMovement { get; set; } = true;
        private Vector3 _autoMovement;

        private Vector3 _velocity;
        public Vector3 Velocity => _velocity;
        private float _verticalVelocity;
        private Vector3 _movementDirection;

        private Entity _entity;
        private EntityStat _statCompo;
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStat>();
        }
        
        public void AfterInitialize()
        {
            _moveSpeed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 1f);
        }

        private void OnDestroy()
        {
            _statCompo.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
        }

        private void HandleMoveSpeedChange(StatSO stat, float currentvalue, float prevvalue)
        {
            _moveSpeed = currentvalue;
        }

        public void SetMovementDirection(Vector2 movementInput)
        {
            _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        }

        private void FixedUpdate()
        {
            CalculateMovement();
            ApplyGravity();
            Move();
        }

        private void CalculateMovement()
        {
            if (CanManualMovement)
            {
                _velocity = parent.transform.rotation * _movementDirection;
                if(_isRunning) _velocity *= _runMoveSpeedMultiply;
                _velocity *= _moveSpeed * 0.05f * Time.fixedDeltaTime;
            }
            else
            {
                _velocity = _autoMovement * Time.fixedDeltaTime;
            }
        }

        private void ApplyGravity()
        {
            if(IsGround && _verticalVelocity < 0)
                _verticalVelocity = -0.03f;
            else 
                _verticalVelocity += gravity * Time.fixedDeltaTime;
            
            _velocity.y = _verticalVelocity;
        }

        private void Move()
        {
            controller.Move(_velocity);
        }

        public void StopImmediately()
        {
            _movementDirection = Vector3.zero;
        }
        
        public void SetAutoMovement(Vector3 autoMovement) => _autoMovement = autoMovement;
        
    }
}