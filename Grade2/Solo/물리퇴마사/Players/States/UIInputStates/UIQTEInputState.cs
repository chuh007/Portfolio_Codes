using _01Scripts.Combat;
using _01Scripts.Core;
using _01Scripts.Entities;
using QTESystem;
using UnityEngine;

namespace _01Scripts.Players.States.UIInputStates
{
    public class UIQTEInputState : UIInputState
    {
        private QTEComponent _qteCompo;
        private PlayerAttackCompo _playerAttackCompo;

        private int _triggerdCount = 0;
        
        public UIQTEInputState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _qteCompo = _player.GetCompo<QTEComponent>();
            _playerAttackCompo = _player.GetCompo<PlayerAttackCompo>();
        }

        public override void Reset()
        {
            base.Reset();
            _qteCompo = _player.GetCompo<QTEComponent>();
            _playerAttackCompo = _player.GetCompo<PlayerAttackCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            PlayerUIInoutComponent.InputUIChanged(ControlUIType.UIQTEInput);
            _player.PlayerBattleInput.OnQTEKeyPressed += HandleQTEPressed;
            _qteCompo.onSuccess.AddListener(HandleSuccess);
            _qteCompo.onFailure.AddListener(HandleFailure);
            _triggerdCount = 0;
            _qteCompo.StartCoroutine(_qteCompo.QTEStart(_playerAttackCompo.currentAttackData.triggerCount));
        }

        private void HandleSuccess()
        {
            _playerAttackCompo.QteSuccess();
            if (_triggerdCount < _playerAttackCompo.currentAttackData.triggerCount - 1)
            {
                _triggerdCount++;
                return;
            }
            _player.ChangeState("ATTACKMOTION");
        }
        
        private void HandleFailure()
        {
            _qteCompo.StopAllCoroutines();
            _player.ChangeState("ATTACKMOTION");
        }

        private void HandleQTEPressed()
        {
            _qteCompo.HandleQTEPressed();
        }

        public override void Exit()
        {
            _player.PlayerBattleInput.OnQTEKeyPressed -= HandleQTEPressed;
            _qteCompo.onSuccess.RemoveListener(HandleSuccess);
            _qteCompo.onFailure.RemoveListener(HandleFailure);
            base.Exit();
        }
    }
}