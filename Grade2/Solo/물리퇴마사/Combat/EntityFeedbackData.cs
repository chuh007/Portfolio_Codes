using _01Scripts.Entities;
using UnityEngine;

namespace _01Scripts.Combat
{
    public class EntityFeedbackData : MonoBehaviour, IEntityComponent
    {
        #region Hit data

        [field: SerializeField] public bool IsLastHitCritical { get; set; } = false;
        [field: SerializeField] public Entity LastEntityWhoHit { get; set; }
        
        #endregion
        
        private Entity _entity;
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }
    }
}