using System;
using Code.Animators;
using Code.Core.EventSystems;
using Code.Core.StatSystem;
using Code.Entities;
using Code.Entities.FSM;
using Code.SkillSystem;
using Code.SkillSystem.Dash;
using UnityEngine;

namespace Code.Players
{
    public class Player : Entity
    {
        [field: SerializeField] public GameEventChannelSO PlayerChannel { get; private set; }
        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] private StateListSO playerFSM;

        private StateMachine _stateMachine;

        [SerializeField] private StatSO jumpCountStat, attackSpeedStat;
        [field: SerializeField] public AnimParamSO ComboCounterParam { get; private set; }

        [field: SerializeField] public GameEventChannelSO UIChannel { get; private set; }

        [SerializeField] private GameObject gameover;
        
        private int _maxJumpCount;
        private int _currentJumpCount;
        private float _coyoteTime = 0;

        public bool CanJump => _currentJumpCount > 0;

        public bool CanCoyoteJump => _coyoteTime < 0.2f;


        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new StateMachine(this, playerFSM);
        }

        protected override void AfterInitialize()
        {
            base.AfterInitialize();
            EntityStat statCompo = GetCompo<EntityStat>();
            statCompo.GetStat(jumpCountStat).OnValueChange += HandleJumpCountChange;
            _currentJumpCount = _maxJumpCount = Mathf.RoundToInt(statCompo.GetStat(jumpCountStat).Value);

            PlayerInput.OnDashKeyPressed += HandleDashKeyPress;
            GetCompo<EntityAnimationTrigger>().OnAnimationEnd += HandleAnimationEnd;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GetCompo<EntityStat>().GetStat(jumpCountStat).OnValueChange -= HandleJumpCountChange;
            PlayerInput.OnDashKeyPressed -= HandleDashKeyPress;
            GetCompo<EntityAnimationTrigger>().OnAnimationEnd -= HandleAnimationEnd;
            
            PlayerInput.ClearSubscription();
        }

        protected override void HandleHit()
        {
        }

        protected override void HandleDead()
        {
            ChangeState("DEAD");
            gameover.SetActive(true);
            Time.timeScale = 0;
        }

        private void HandleAnimationEnd() => _stateMachine.CurrentState.AnimationEndTrigger();

        private void HandleDashKeyPress()
        {
            float facingDirection = GetCompo<EntityRenderer>().FacingDirection;
            if (GetCompo<EntityMover>().IsWallDetected(facingDirection)) return;
            
            if(GetCompo<SkillCompo>().GetSkill<DashSkill>().AttemptUseSkill())
                ChangeState("DASH");
        }

        private void HandleJumpCountChange(StatSO stat, float current, float previous)
            => _maxJumpCount = Mathf.RoundToInt(current);
        
        public void DecreaseJumpCount() => _currentJumpCount--;
        public void ResetJumpCount() => _currentJumpCount = _maxJumpCount;

        public void UpdateCoyoteTime() => _coyoteTime += Time.deltaTime;

        public void ResetCoyoteTime() =>_coyoteTime = 0;
        
        private void Start()
        {
            _stateMachine.ChangeState("IDLE");
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        public void ChangeState(string newStateName) => _stateMachine.ChangeState(newStateName);
        
    }
}