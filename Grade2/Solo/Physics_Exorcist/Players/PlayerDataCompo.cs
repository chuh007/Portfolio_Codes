using System;
using System.Collections.Generic;
using _01Scripts.Combat;
using _01Scripts.Core.EventSystem;
using _01Scripts.Entities;
using Code.Core.GameSystem;
using UnityEngine;

namespace _01Scripts.Players
{
     public class PlayerDataCompo : MonoBehaviour, IEntityComponent, ISavable
    {
        [SerializeField] private GameEventChannelSO playerChannel;

        public Vector3 position;
        public int currentExp;
        public int level;
        public float health;
        
        private Player _player;
        private EntityHealthComponent _healthComponent;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            _healthComponent = _player.GetCompo<EntityHealthComponent>();
            playerChannel.AddListener<AddEXPEvent>(HandleAddExp);
            _healthComponent.currentHpValueChangeEvent.AddListener(HandleHpChange);
        }

        private void OnDestroy()
        {
            _healthComponent.currentHpValueChangeEvent.RemoveListener(HandleHpChange);
            playerChannel.RemoveListener<AddEXPEvent>(HandleAddExp);
        }

        private void HandleHpChange(float value)
        {
            health = value;
        }

        private void HandleAddExp(AddEXPEvent addExpEvt) => AddExp(addExpEvt.exp);

        public void AddExp(int amount)
        {
            currentExp += amount;
        }

        #region SaveData Logic
        
        [field: SerializeField] public SaveIdSO SaveID { get; private set; }
        
        [Serializable]
        public struct PlayerSaveData
        {
            public Vector3 position;
            public int currentExp;
            public int level;
            public float health;
            public List<EntityStat.StatSaveData> stats;
        }
        
        public string GetSaveData()
        {
            
            PlayerSaveData data = new PlayerSaveData
            {
                position = position, currentExp = currentExp, level = level, health = health,
                stats = _player.GetCompo<EntityStat>().GetSaveData()
            };
            if (_player.playerType == PlayerType.Search)
            {
                data.position = _player.transform.position;
                position = _player.transform.position;
            }
            return JsonUtility.ToJson(data);
        }

        public void RestoreData(string loadedData)
        {
            PlayerSaveData loadData = JsonUtility.FromJson<PlayerSaveData>(loadedData);
            position = loadData.position;
            if (_player.playerType == PlayerType.Search)
            {
                _player.GetComponent<CharacterController>().enabled = false;
                _player.transform.position = position;
                _player.GetComponent<CharacterController>().enabled = true;
            }
            health = loadData.health;
            if(health == 0) health = _healthComponent.maxHealth;
            _player.GetCompo<EntityHealthComponent>().RestoreHealth(health);
            currentExp = loadData.currentExp;
            level = loadData.level;

            if(loadData.stats != null)
                _player.GetCompo<EntityStat>().RestoreData(loadData.stats);
        }
        
        #endregion
        
    }
}