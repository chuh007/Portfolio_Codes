using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect.EffectData
{
    /// <summary>
    /// 도트뎀, 도트힐 등 일정 간격으로 발동하는 이펙트들
    /// </summary>
    public interface ITickableEffect
    {
        float TickDelay { get; }
        void OnTick(Entity target);
    }
}