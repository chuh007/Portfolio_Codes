using ChipmunkKingdoms.Scripts.Utility;
using Chuh007Lib.StatSystem;
using UnityEngine;

namespace Work.CHUH._01Scripts.Entities
{
    public class EntityMover : MonoBehaviour, IContainerComponent, IAfterInitailze
    {
        [SerializeField] private StatSO speedStat;

        private Entity _entity;
        private EntityStat _statCompo;
        private Rigidbody2D _rbCompo;
        private float _speed;
        
        public ComponentContainer ComponentContainer { get; set; }
        public void OnInitialize(ComponentContainer componentContainer)
        {

        }
        
        public void AfterInitailized()
        {
            _entity = ComponentContainer.Get<Entity>(true);
            _rbCompo = _entity.GetComponent<Rigidbody2D>();
            _statCompo = ComponentContainer.Get<EntityStat>();
            _speed = _statCompo.GetStat(speedStat).Value;
            _statCompo.GetStat(speedStat).OnValueChanged += HandleSpeedValueChange;
        }

        private void HandleSpeedValueChange(StatSO stat, float currentvalue, float prevvalue)
        {
            _speed = currentvalue;
        }

        public void Move(Vector2 dir)
        {
            _rbCompo.linearVelocity = dir * _speed;
            _entity.transform.rotation = dir.x > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            if (gameObject.name == "EntityMovers")
            {
                Debug.Log($"Move Speed : {speedStat.Value}");
            }
        }

        public void Stop()
        {
            _rbCompo.linearVelocity = Vector2.zero;
        }

    }
}