using Code.Animators;
using Code.Core.EventSystems;
using Code.Entities;
using Code.Entities.FSM;
using Code.SkillSystem;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerSkillChargeState : EntityState
    {
        private Player _player;
        private EntityMover _mover;
        
        public PlayerSkillChargeState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately(false);
            _player.PlayerChannel.AddListener<SkillChargedEvent>(HandleSkillChargedEvent);
            _player.PlayerChannel.AddListener<SkillChargeEndEvent>(HandleSkillChargeEndEvent);
            _player.PlayerInput.OnSkillKeyPressed += HandleSkillKeyPressed;
        }

        public override void Exit()
        {
            _player.PlayerChannel.RemoveListener<SkillChargedEvent>(HandleSkillChargedEvent);
            _player.PlayerChannel.RemoveListener<SkillChargeEndEvent>(HandleSkillChargeEndEvent);
            _player.PlayerInput.OnSkillKeyPressed -= HandleSkillKeyPressed;
            base.Exit();
        }

        private void HandleSkillChargeEndEvent(SkillChargeEndEvent endEvt)
        {
            _player.ChangeState("IDLE");
        }

        private void HandleSkillKeyPressed(bool isPressed)
        {
            if (isPressed) return;
            
            SkillCompo skillCompo = _player.GetCompo<SkillCompo>();
            if (skillCompo.activeSkill == null) return;

            //현재 가지고 있는 스킬이 릴리즈 가능한 스킬이라면 릴리즈 명령을 내려준다.
            if (skillCompo.activeSkill is IReleasable releasable)
            {
                releasable.ReleaseSkill();
            }
        }

        private void HandleSkillChargedEvent(SkillChargedEvent evt)
        {
            Debug.Log($"player state skill counted <color=green>{evt.chargeCount}</color>");
        }
    }
}