using Chuh007Lib.Bus;
using _Code.LCH._02.Scripts.Combat;
using UnityEngine;

namespace _Work.CHUH.Code.Core.Events
{
    public struct EnemyHitEvent : IEvent
    {
        public Vector3 Pos;
        public float Damage;
        public DamageType DamageType;
        public float DamageTypeMultiplier;

        public EnemyHitEvent(
            Vector3 pos,
            float damage,
            DamageType damageType = DamageType.Physical,
            float damageTypeMultiplier = 1f)
        {
            Pos = pos;
            Damage = damage;
            DamageType = damageType;
            DamageTypeMultiplier = damageTypeMultiplier;
        }
    }
}
