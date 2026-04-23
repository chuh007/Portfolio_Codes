using _01Scripts.Combat;
using _01Scripts.Core;
using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace _01Scripts.Players.States.UIInputStates
{
    public class UIUseItemInputState : UIInputState
    {
        private PlayerInvenData _playerInvenData;
        private EntityHealthComponent _healthComponent;
        private EntityStat _statCompo;
        private PlayerBattleCompo _playerBattleCompo;
        
        public UIUseItemInputState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _playerInvenData = entity.GetCompo<PlayerInvenData>();
            _healthComponent = entity.GetCompo<EntityHealthComponent>();
            _statCompo = entity.GetCompo<EntityStat>();
            _playerBattleCompo = entity.GetCompo<PlayerBattleCompo>();
        }

        public override void Reset()
        {
            base.Reset();
            _playerInvenData = _player.GetCompo<PlayerInvenData>();
            _healthComponent = _player.GetCompo<EntityHealthComponent>();
            _statCompo = _player.GetCompo<EntityStat>();
            _playerBattleCompo = _player.GetCompo<PlayerBattleCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _player.PlayerBattleInput.OnCancelOrESCKeyPressed += HandleCancelOrEscKeyPressed;
            _player.PlayerBattleInput.OnSelect1KeyPressed += HandleSelect1KeyPressed;
            _player.PlayerBattleInput.OnSelect2KeyPressed += HandleSelect2KeyPressed;
            PlayerUIInoutComponent.InputUIChanged(ControlUIType.UIUseItemSelect);
            InventoryDataEvent evt = InventoryEvents.InventoryDataEvent;
            evt.slotCount = 2;
            evt.items = _playerInvenData.inventory;
            _playerInvenData.inventoryChannel.RaiseEvent(evt);
        }

        private void HandleCancelOrEscKeyPressed()
        {
            _player.ChangeState("UISELECT");
        }

        private void HandleSelect1KeyPressed()
        {
            if (_playerInvenData.inventory[0].data.itemName == "회복약")
            {
                if (_playerInvenData.CanRemoveItem(_playerInvenData.healItemData))
                {
                    _playerInvenData.RemoveItem(_playerInvenData.healItemData);
                    _healthComponent.ApplyHeal(20);
                    TurnEndEvent evt = TurnEvents.TurnEndEvent;
                    _player.TurnChannel.RaiseEvent(evt);
                    _playerBattleCompo.PlayHealEffect();
                    _player.ChangeState("UIBLOCK");
                }
            }
            else
            {
                if (_playerInvenData.CanRemoveItem(_playerInvenData.attackItemData))
                {
                    _playerInvenData.RemoveItem(_playerInvenData.attackItemData);
                    _statCompo.AddModifier(_statCompo.GetStat("AttackDamage"), "Buff", 10f);
                    TurnEndEvent evt = TurnEvents.TurnEndEvent;
                    _player.TurnChannel.RaiseEvent(evt);
                    _playerBattleCompo.AttackUpEffect();
                    _player.ChangeState("UIBLOCK");
                }
            }
        }

        private void HandleSelect2KeyPressed()
        {
            if (_playerInvenData.inventory[1].data.itemName == "회복약")
            {
                if (_playerInvenData.CanRemoveItem(_playerInvenData.healItemData))
                {
                    _playerInvenData.RemoveItem(_playerInvenData.healItemData);
                    _healthComponent.ApplyHeal(20);
                    TurnEndEvent evt = TurnEvents.TurnEndEvent;
                    _player.TurnChannel.RaiseEvent(evt);
                    _playerBattleCompo.PlayHealEffect();
                    _player.ChangeState("UIBLOCK");
                }
            }
            else
            {
                if (_playerInvenData.CanRemoveItem(_playerInvenData.attackItemData))
                {
                    _playerInvenData.RemoveItem(_playerInvenData.attackItemData);
                    _statCompo.AddModifier(_statCompo.GetStat("AttackDamage"), "Buff", 10f);
                    TurnEndEvent evt = TurnEvents.TurnEndEvent;
                    _player.TurnChannel.RaiseEvent(evt);
                    _playerBattleCompo.AttackUpEffect();
                    _player.ChangeState("UIBLOCK");
                }
            }
        }
        
        public override void Exit()
        {
            _player.PlayerBattleInput.OnCancelOrESCKeyPressed -= HandleCancelOrEscKeyPressed;
            _player.PlayerBattleInput.OnSelect1KeyPressed -= HandleSelect1KeyPressed;
            _player.PlayerBattleInput.OnSelect2KeyPressed -= HandleSelect2KeyPressed;
            base.Exit();
        }
    }
}