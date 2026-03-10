using Code.Animators;
using Code.Combats.Onset;
using Code.Core.EventSystems;
using Code.Entities;
using Code.Entities.FSM;
using Code.SkillSystem;
using UnityEngine;

namespace Code.Players.States
{
    public abstract class PlayerGroundState : PlayerState
    {
        protected EntityMover _mover;

        public PlayerGroundState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _player.ResetCoyoteTime();
            _player.PlayerInput.OnJumpKeyPressed += HandleJumpKeyPress;
            _player.PlayerInput.OnAttackKeyPressed += HandleAttackKeyPress;
            _player.PlayerInput.OnCounterKeyPressed += HandleCounterKeyPress;
            _player.PlayerInput.OnSkillKeyPressed += HandleSkillKeyPress;
        }


        public override void Update()
        {
            base.Update();
            if (_mover.IsGroundDetected() == false && _mover.CanManualMove)
            {
                _player.ChangeState("FALL");
            }
        }

        public override void Exit()
        {
            _player.PlayerInput.OnJumpKeyPressed -= HandleJumpKeyPress;
            _player.PlayerInput.OnAttackKeyPressed -= HandleAttackKeyPress;
            _player.PlayerInput.OnCounterKeyPressed -= HandleCounterKeyPress;
            _player.PlayerInput.OnSkillKeyPressed -= HandleSkillKeyPress;
            base.Exit();
        }

        private void HandleSkillKeyPress(bool isPressed)
        {
            Skill activeSkill = _player.GetCompo<SkillCompo>().activeSkill;
            if (activeSkill == null) return;
            
            if (isPressed && activeSkill.AttemptUseSkill())
            {
                if(activeSkill is IReleasable)
                    _player.ChangeState("SKILL_CHARGE");
                else 
                    _player.ChangeState("SKILL_USE");
            }
        }

        private void HandleCounterKeyPress()
        {
            _player.ChangeState("COUNTER_ATTACK");
        }

        protected virtual void HandleAttackKeyPress()
        {
            if(_mover.IsGroundDetected())
                _player.ChangeState("ATTACK");
        }

        private void HandleJumpKeyPress()
        {
            if(_mover.IsGroundDetected())
                _player.ChangeState("JUMP");
        }
        
    }
}
