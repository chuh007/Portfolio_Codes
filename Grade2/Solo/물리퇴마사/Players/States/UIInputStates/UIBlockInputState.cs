using _01Scripts.Core;
using _01Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace _01Scripts.Players.States.UIInputStates
{
    public class UIBlockInputState : UIInputState
    {
        private PlayerCostCompo _costCompo;
        private PlayerBattleCompo _battleCompo;
        private int hitAnimationHash = Animator.StringToHash("HIT");

        private bool _canBlock = false;
        
        public UIBlockInputState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _costCompo = _player.GetCompo<PlayerCostCompo>();
            _battleCompo = _player.GetCompo<PlayerBattleCompo>();
        }

        public override void Reset()
        {
            base.Reset();
            _costCompo = _player.GetCompo<PlayerCostCompo>();
            _battleCompo = _player.GetCompo<PlayerBattleCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _canBlock = true;
            PlayerUIInoutComponent.InputUIChanged(ControlUIType.UIBlockInput);
            _player.OnDefense.AddListener(HandleDefense);
            _player.OnHit.AddListener(HandleHit);
            _player.PlayerBattleInput.OnBlockKeyPressed += HandleBlockPressed;

        }

        private void HandleHit()
        {
            _entityAnimator.SetParam(hitAnimationHash);
        }

        private void HandleDefense()
        {
            _canBlock = true;
            _costCompo.PlusCost(1);
        }

        private void HandleBlockPressed()
        {
            if(!_canBlock) return;
            _canBlock = false;
            DOVirtual.DelayedCall(0.05f, () =>
            {
                _battleCompo.BlockStart();
                _entity.IsDefense = true;
            });
            DOVirtual.DelayedCall(0.5f, () => _canBlock = true);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                _battleCompo.BlockEnd();
                _entity.IsDefense = false;
            });
        }

        public override void Exit()
        {
            _costCompo.PlusCost(1);
            _player.OnDefense.RemoveListener(HandleDefense);
            _player.PlayerBattleInput.OnBlockKeyPressed -= HandleBlockPressed;
            _player.OnHit.RemoveListener(HandleHit);
            base.Exit();
        }
    }
}