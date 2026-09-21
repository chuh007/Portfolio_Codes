using _Code.LCH._02.Scripts.Combat;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    // 도트뎀같은 이펙트가 상속받음
    public interface IDamagingEffect
    {
        public void DamageableUpdate(Entity target);
    }
}