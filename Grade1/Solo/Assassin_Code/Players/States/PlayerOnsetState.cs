using Code.Animators;
using Code.Core.EventSystems;
using Code.Entities;
using Code.Entities.FSM;
using DG.Tweening;
using System;
using UnityEngine;

namespace Code.Players.States
{
    public class PlayerOnsetState : PlayerState
    {
        private PlayerAttackCompo _attackCompo;
        private Transform target;

        public PlayerOnsetState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
        {
            _attackCompo = entity.GetCompo<PlayerAttackCompo>();
            _player.PlayerChannel.AddListener<OnsetTargetEvent>(HandleOnSet);
        }

        public override void Enter()
        {
            base.Enter();
            _renderer.FlipController(target.transform.position.x - _player.transform.position.x);
            Vector2 dir = target.position - _player.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            _player.transform.DOMove(new Vector2(target.localPosition.x,
                target.position.y), 0.25f).OnComplete(() =>
                {
                    _attackCompo.OnsetAttack(target);

                    _player.ChangeState("IDLE");
                });
        }

        private void HandleOnSet(OnsetTargetEvent evt)
        {
            target = evt.target;
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}

