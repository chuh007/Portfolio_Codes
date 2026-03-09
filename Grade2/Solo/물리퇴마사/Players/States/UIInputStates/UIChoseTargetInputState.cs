using _01Scripts.Combat;
using _01Scripts.Core;
using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Players.States.UIInputStates
{
    public class UIChoseTargetInputState : UIInputState
    {
        private PlayerAttackCompo _attackCompo;
        private PlayerTargetSelector _targetSelector;
        private PlayerCostCompo _costCompo;
        
        public UIChoseTargetInputState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _attackCompo = entity.GetCompo<PlayerAttackCompo>();
            _targetSelector = entity.GetCompo<PlayerTargetSelector>();
            _costCompo = entity.GetCompo<PlayerCostCompo>();
        }
        
        public override void Enter()
        {
            base.Enter();
            _player.PlayerBattleInput.OnCancelOrESCKeyPressed += HandleCancelOrEscKeyPressed;
            _player.PlayerBattleInput.OnSelect2KeyPressed += HandleSelectTarget;
            PlayerUIInoutComponent.InputUIChanged(ControlUIType.UIChoseTarget);
        }

        
        private void HandleSelectTarget()
        {
            if (_targetSelector.CurrentTarget == null)
            {
                _targetSelector.ReGetEntity();
            }
            Entity target = _targetSelector.CurrentTarget;
            _attackCompo.SetTarget(target);
            _costCompo.SpendCost(_attackCompo.currentAttackData.cost);
            _player.ChangeState("UIQTEINPUT");
        }
        
        private void HandleCancelOrEscKeyPressed()
        {
            _player.ChangeState("UIATTACKSELECT");
        }

        public override void Exit()
        {
            _player.PlayerBattleInput.OnCancelOrESCKeyPressed -= HandleCancelOrEscKeyPressed;
            _player.PlayerBattleInput.OnSelect2KeyPressed -= HandleSelectTarget;
            base.Exit();
        }
    }
}