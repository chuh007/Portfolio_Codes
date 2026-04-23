using _01Scripts.Combat;
using _01Scripts.Core;
using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Players.States.UIInputStates
{
    public class UIAttackSelectInputState : UIInputState
    {
        private PlayerAttackCompo _playerAttackCompo;
        private PlayerCostCompo _playerCostCompo;
        public UIAttackSelectInputState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _playerAttackCompo = _player.GetCompo<PlayerAttackCompo>();
            _playerCostCompo = _player.GetCompo<PlayerCostCompo>();
        }

        public override void Reset()
        {
            base.Reset();
            _playerAttackCompo = _player.GetCompo<PlayerAttackCompo>();
            _playerCostCompo = _player.GetCompo<PlayerCostCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _player.PlayerBattleInput.OnCancelOrESCKeyPressed += HandleCancelOrEscKeyPressed;
            _player.PlayerBattleInput.OnSelect1KeyPressed += HandleSelect1KeyPressed;
            _player.PlayerBattleInput.OnSelect2KeyPressed += HandleSelect2KeyPressed;
            _player.PlayerBattleInput.OnSelect3KeyPressed += HandleSelect3KeyPressed;
            PlayerUIInoutComponent.InputUIChanged(ControlUIType.UIAttackSelect);

        }


        private void HandleSelect1KeyPressed()
        {
            _playerAttackCompo.currentAttackData = _playerAttackCompo.GetAttackData("NomalAttack");
            _player.ChangeState("UICHOSETARGET");
        }

        private void HandleSelect2KeyPressed()
        {
            if(!_playerCostCompo.TrySpendCost(4)) return;
            _playerAttackCompo.currentAttackData = _playerAttackCompo.GetAttackData("SpinAttack");
            _player.ChangeState("UICHOSETARGET");

        }

        private void HandleSelect3KeyPressed()
        {
            if(!_playerCostCompo.TrySpendCost(9)) return;
            _playerAttackCompo.currentAttackData = _playerAttackCompo.GetAttackData("SpecialAttack");
            _player.ChangeState("UICHOSETARGET");
        }
        
        private void HandleCancelOrEscKeyPressed()
        {
            _player.ChangeState("UISELECT");
        }
        
        public override void Exit()
        {
            _player.PlayerBattleInput.OnCancelOrESCKeyPressed -= HandleCancelOrEscKeyPressed;
            _player.PlayerBattleInput.OnSelect1KeyPressed -= HandleSelect1KeyPressed;
            _player.PlayerBattleInput.OnSelect2KeyPressed -= HandleSelect2KeyPressed;
            _player.PlayerBattleInput.OnSelect3KeyPressed -= HandleSelect3KeyPressed;
            base.Exit();
        }

    }
}