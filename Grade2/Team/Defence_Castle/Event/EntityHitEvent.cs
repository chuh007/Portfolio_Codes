using Code.CoreSystem;
using UnityEngine;

namespace Work.CHUH._01Scripts.Event
{
    public class EntityHitEvent : IEvent
    {
        public Vector3 pos;
        public float damage;

        public EntityHitEvent(float damage, Vector3 pos)
        {
            this.damage = damage;
            this.pos = pos;
        }
    }
}