using Code.Animators;
using Code.Core.EventSystems;
using Code.Entities;
using Code.Entities.FSM;
using Code.SkillSystem;
using Code.SkillSystem.PlayerClone;
using DG.Tweening;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerDashState : EntityState
    {
        private Player _player;
        private EntityMover _mover;

        private readonly float _dashDistance = 4.5f, _dashTime = 0.25f;
        
        public PlayerDashState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _player = entity as Player;
            _mover = entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();
            Vector2 playerInput = _player.PlayerInput.InputDirection;
            Vector2 dashDirection = playerInput.magnitude > 0.05f ? playerInput : _renderer.transform.right;

            _mover.CanManualMove = false;//이동 잠궈주고
            _mover.SetGravityScale(0f);
            _mover.StopImmediately(true);
            
            Vector3 destination = _player.transform.position + (Vector3)dashDirection * _dashDistance;
            float dashTime = _dashTime;

            if (_mover.CheckColliderInFront(dashDirection, _dashDistance,  out float distance))
            {
                destination = _player.transform.position + (Vector3)dashDirection * distance;//수정된 좌표로 정정
                dashTime = distance * _dashTime / _dashDistance;
            }
            _player.transform.DOMove(destination, dashTime).SetEase(Ease.OutQuad).OnComplete(EndDash);

            _player.PlayerChannel.RaiseEvent(PlayerEvents.DashStartEvent);
            //_player.GetCompo<SkillCompo>().GetSkill<CloneSkill>()?.CreateCloneOnDashStart();
        }

        private void EndDash()
        {
            _player.ChangeState("IDLE");
        }

        public override void Exit()
        {
            _player.PlayerChannel.RaiseEvent(PlayerEvents.DashEndEvent);
            //_player.GetCompo<SkillCompo>().GetSkill<CloneSkill>()?.CreateCloneOnDashEnd();
            
            _mover.StopImmediately(false);
            _mover.CanManualMove = true;
            _mover.SetGravityScale(1f);
            base.Exit();
        }
    }
}