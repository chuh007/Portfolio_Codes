using _01Scripts.Entities;
using _01Scripts.Players;
using UnityEngine;

namespace _01Scripts.TurnSystem
{
    public class PlayerTurnComponent : EntityTurnComponent
    {
        [SerializeField] private Sprite icon;
        
        private Player _player;
        
        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            _player = entity as Player;
            Icon = icon;
            Name = "플레이어";
        }

        public override void TurnAction()
        {
            base.TurnAction();
            if(_player.IsDead) return;
            _player.ChangeState("UISELECT");
        }
    }
}