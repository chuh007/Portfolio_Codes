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
        public AbstractEffectDataSO EffectData { get; protected set; }
        public Entity EffectTarget { get; }
        public Entity Source { get; protected set; }     // 누가 걸었는지 (킬 판정, 흡혈 등에 필요)
        
        public float Duration { get; protected set; }
        public int CurrentStack { get; private set; }
        public virtual int MaxStack => Mathf.Max(1, EffectData.maxStack);
        public bool IsFullStack => CurrentStack >= MaxStack;
        public bool IsExpired => Duration <= 0f;

        private float _tickTimer;
        private bool _isRemoved;

        protected virtual float TickDelay => EffectData is ITickableEffect tickable ? tickable.TickDelay : 0f;

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

        public virtual void RefreshFrom(EntityEffect effect)
        {
            RefreshDuration();
            if (effect.EffectData.stackPolicy == StackPolicy.Stack)
                AddStack();
        }

        public virtual void Activate()
        {
            if (EffectData is IDurationEffect durationEffect)
                durationEffect.ActiveEffect(EffectTarget);
        }

        public virtual void Deactivate()
        {
            _isRemoved = true;
            Duration = 0f;
            if (EffectData is IDurationEffect durationEffect)
                durationEffect.UnActiveEffect(EffectTarget);
        }

        public virtual void OnTargetDeath() { }

        public virtual bool Matches(AbstractEffectDataSO data)
        {
            return !string.IsNullOrEmpty(data.effectId)
                ? EffectData.effectId == data.effectId
                : EffectData == data;
        }

        public bool AddStack(int amount = 1)
        {
            int oldStack = CurrentStack;
            CurrentStack = Mathf.Min(CurrentStack + amount, MaxStack);
            return CurrentStack != oldStack;
        }

        public void UpdateEffect()
        {
            UpdateEffect(Time.deltaTime);
        }

        public void UpdateEffect(float deltaTime)
        {
            if (_isRemoved || IsExpired || EffectTarget == null || EffectTarget.IsDead) return;

            float elapsed = Mathf.Min(Mathf.Max(0f, deltaTime), Duration);
            float delay = TickDelay;
            if (delay > 0f)
            {
                _tickTimer += elapsed;
                while (_tickTimer >= delay && !_isRemoved && !EffectTarget.IsDead)
                {
                    _tickTimer -= delay;
                    OnTick();
                }
            }
            if (!_isRemoved)
                Duration = Mathf.Max(0f, Duration - elapsed);
        }

        protected virtual void OnTick()
        {
            if (EffectData is ITickableEffect tickable)
                tickable.OnTick(EffectTarget);
        }
    }
}
