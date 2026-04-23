using System;
using _01Scripts.Combat;
using _01Scripts.Entities;
using _01Scripts.TurnSystem;
using Chuh007Lib.StatSystem;
using Code.Core.GameSystem;
using UnityEngine;

namespace _01Scripts.Enemies
{
    public class EnemyDataCompo : MonoBehaviour, IEntityComponent, ISavable
    {
        [Header("Data Settings")]
        public bool useThisData = false;
        [SerializeField] private StatSO hpStat;
        [SerializeField] private StatSO speedStat;
        [SerializeField] private StatSO attackDamageStat;
        private EnemyTurnComponent _enemyTurnComponent;
        
        [Header("Temp")]
        [SerializeField] private EnemyDataSO enemyData;
        
        private Enemy _enemy;
        
        private EntityStat _entityStat;
        private EntityAttackCompo _entityAttackCompo;
        
        public void Initialize(Entity entity)
        {
            _enemy = entity as Enemy;
            _entityStat = entity.GetCompo<EntityStat>();
            _entityAttackCompo = entity.GetCompo<EntityAttackCompo>(true);
            _enemyTurnComponent = entity.GetCompo<EnemyTurnComponent>();
        }

        [field: SerializeField] public SaveIdSO SaveID { get; private set; }
        
        [Serializable]
        public struct EnemySaveData
        {
            public EnemyDataSO enemyData;
        }
        
        public string GetSaveData()
        {
            if (!useThisData) return "";
            EnemySaveData data = new EnemySaveData
            {
                enemyData = enemyData
            };
            return JsonUtility.ToJson(data);
        }

        public void RestoreData(string loadedData)
        {
            EnemySaveData loadData = JsonUtility.FromJson<EnemySaveData>(loadedData);
            
            enemyData = loadData.enemyData;
            ApplyEnemyData(enemyData);
        }

        private void ApplyEnemyData(EnemyDataSO data)
        {
            if(_enemy == null) return;
            foreach (Transform child in _enemy.GetCompo<EntityAnimator>().transform)
            {
                child.gameObject.SetActive(false);
                if (child.name == data.visualName)
                {
                    child.gameObject.SetActive(true);
                }
            }
            _entityStat.SetBaseValue(hpStat ,data.health);
            _entityStat.SetBaseValue(speedStat ,data.speed);
            _entityStat.SetBaseValue(attackDamageStat ,data.damage);
            _entityAttackCompo.attackDataList = data.attackDataList;
            _enemyTurnComponent.Name = data.enemyName;
            _enemyTurnComponent.Icon = data.icon;
            if(data.isBoss) _enemy.SetBoss();
        }
    }
}