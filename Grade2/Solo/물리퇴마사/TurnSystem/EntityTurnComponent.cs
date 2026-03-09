using System;
using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;
using UnityEngine.UI;

namespace _01Scripts.TurnSystem
{
    public abstract class EntityTurnComponent : MonoBehaviour, IEntityComponent, ITurnActor
    {
        [SerializeField] protected StatSO speedStat;
        [SerializeField] private GameEventChannelSO uiChannel;
        
        private Entity _entity;


        private void Awake()
        {
            uiChannel.AddListener<FadeCompleteEvent>(HandleFadeCompleteApplyData);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<FadeCompleteEvent>(HandleFadeCompleteApplyData);
        }

        public virtual void Initialize(Entity entity)
        {
            _entity = entity;
            Speed = (int)entity.GetCompo<EntityStat>().GetStat(speedStat).Value;
        }
        
        private void HandleFadeCompleteApplyData(FadeCompleteEvent obj)
        {
            Speed = (int)_entity.GetCompo<EntityStat>().GetStat(speedStat).Value;
        }

        public string Name { get; set; }
        
        public Sprite Icon { get; set; }
        public int Speed { get; set; }
        public int ActionValue { get; set; } = 0;
        public virtual void TurnAction()
        {
            
        }
    }
}