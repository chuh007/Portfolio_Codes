using UnityEngine;

namespace Code.Entities
{
    public class EntityFeedbackData : MonoBehaviour, IEntityComponent
    {
        #region Hit data

        [field: SerializeField] public bool IsLastHitCritical { get; set; } = false;
        [field: SerializeField] public bool IsLastHitPowerAttack { get; set; } = false;
        [field: SerializeField] public Vector2 LastAttackDirection { get; set; }
        [field: SerializeField] public Entity LastEntityWhoHit { get; set; }
        
        #endregion
        
        private Entity _entity;
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }
    }
}