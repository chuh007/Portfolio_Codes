using _01Scripts.Entities;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace _01Scripts.Players
{
    public class PlayerStatAdder : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private StatSO hpStat, attackStat, speedStat;
        [SerializeField] private GameObject uiObj;
        private Entity _entity;
        private EntityStat _entityStat;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _entityStat = _entity.GetCompo<EntityStat>();
        }

        public void AddAttackStat(float value)
        {
            float stat = _entityStat.GetStat(attackStat).Value;
            _entityStat.SetBaseValue(attackStat, stat + value);
            uiObj.SetActive(false);
        }
        
        public void AddHealthStat(float value)
        {
            float stat = _entityStat.GetStat(hpStat).Value;
            _entityStat.SetBaseValue(hpStat, stat + value);
            uiObj.SetActive(false);
        }
        
        public void AddSpeedStat(float value)
        {
            float stat = _entityStat.GetStat(speedStat).Value;
            _entityStat.SetBaseValue(speedStat, stat + value);
            uiObj.SetActive(false);
        }
    }
}