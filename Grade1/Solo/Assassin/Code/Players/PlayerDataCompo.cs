using System;
using System.Collections.Generic;
using Code.Core.EventSystems;
using Code.Core.GameSystem;
using Code.Entities;
using UnityEngine;

namespace Code.Players
{
    public class PlayerDataCompo : MonoBehaviour, IEntityComponent, ISavable
    {
        [SerializeField] private GameEventChannelSO playerChannel;

        public int currentExp;
        public int level;
        public int skillPoints;
        public int gold;
        
        private Player _player;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            playerChannel.AddListener<AddEXPEvent>(HandleAddExp);
        }

        private void OnDestroy()
        {
            playerChannel.RemoveListener<AddEXPEvent>(HandleAddExp);
        }

        private void HandleAddExp(AddEXPEvent addExpEvt) => AddExp(addExpEvt.exp);

        public void AddExp(int amount)
        {
            currentExp += amount;
            //나중에 여기서 레벨업이벤트도 발행하고 처리한다.
            //연호, 호성이에게 영상편지
            // 그동안 즐거웠다~
        }

        #region SaveData Logic
        
        [field: SerializeField] public SaveIdSO SaveID { get; private set; }
        
        [Serializable]
        public struct PlayerSaveData
        {
            public int currentExp;
            public int level;
            public int skillPoints;
            public int gold;
            public List<EntityStat.StatSaveData> stats;
        }
        
        public string GetSaveData()
        {
            
            PlayerSaveData data = new PlayerSaveData
            {
                currentExp = currentExp, level = level, skillPoints = skillPoints, gold = gold,
                stats = _player.GetCompo<EntityStat>().GetSaveData()
            };
            return JsonUtility.ToJson(data);
        }

        public void RestoreData(string loadedData)
        {
            PlayerSaveData loadData = JsonUtility.FromJson<PlayerSaveData>(loadedData);
            currentExp = loadData.currentExp;
            level = loadData.level;
            skillPoints = loadData.skillPoints;
            gold = loadData.gold;
            
            if(loadData.stats != null)
                _player.GetCompo<EntityStat>().RestoreData(loadData.stats);
        }
        
        #endregion
        
    }
}