using _Work.CHUH.Code.EntityPlus.Effect.EffectData;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    /// <summary>
    /// Entity에 붙는 이펙트 런타임 인스턴스
    /// 도트뎀, 버프 등등이 들어간다.
    /// </summary>
    public class EntityEffect
    {
        public AbstractEffectDataSO EffectData { get; }
        public Entity EffectTarget { get; }
        public Entity Source { get; }     // 누가 걸었는지 (킬 판정, 흡혈 등에 필요)
        
        public float Duration { get; private set; }
        public int CurrentStack { get; private set; }
        public int MaxStack => EffectData.maxStack;
        public bool IsFullStack => CurrentStack >= MaxStack;
        public bool IsExpired => Duration <= 0f;

        private float _tickTimer;

        public EntityEffect(AbstractEffectDataSO effectData, Entity target, Entity source = null)
        {
            EffectData = effectData;
            EffectTarget = target;
            Source = source;
            Duration = effectData.duration;
            CurrentStack = 1;
        }

        public void RefreshDuration()
        {
            Duration = EffectData.duration;
        }

        public bool AddStack(int amount = 1)
        {
            int oldStack = CurrentStack;
            CurrentStack = Mathf.Min(CurrentStack + amount, MaxStack);
            return CurrentStack != oldStack;
        }

        public void UpdateEffect()
        {
            Duration -= Time.deltaTime;

            if (EffectData is ITickableEffect tickable)
            {
                _tickTimer += Time.deltaTime;
                while (_tickTimer >= tickable.TickDelay)
                {
                    _tickTimer -= tickable.TickDelay;
                    tickable.OnTick(EffectTarget);
                }
            }
        }
    }
}