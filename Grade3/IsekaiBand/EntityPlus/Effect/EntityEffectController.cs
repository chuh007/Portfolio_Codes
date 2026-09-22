using System.Collections.Generic;
using _Work.CHUH.Code.EntityPlus.Effect.EffectData;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    /// <summary>
    /// Entity한테 적용되는 효과들 관리해줌
    /// </summary>
    public class EntityEffectController : MonoBehaviour, IEntityComponent, IEffectHandler
    {
        private Entity _entity;
        private List<EntityEffect> _effects;
        private List<EntityEffect> _toRemove;   
        
        public void Initialize(Entity entity)
        {
            _effects = new List<EntityEffect>();
            _toRemove = new List<EntityEffect>();
            _entity = entity;
        }

        private void Update()
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                _effects[i].UpdateEffect();
                if (_effects[i].IsExpired)
                    _toRemove.Add(_effects[i]);
            }

            for (int i = 0; i < _toRemove.Count; i++)
                RemoveEffectInternal(_toRemove[i]);
            _toRemove.Clear();
        }

        public void AddEffect(AbstractEffectDataSO effectData)
        {
            AddEffect(effectData, null);
        }

        public void AddEffect(AbstractEffectDataSO effectData, Entity source)
        {
            EntityEffect existing = FindEffect(effectData);

            if (existing != null)
            {
                switch (effectData.stackPolicy)
                {
                    case StackPolicy.Ignore:
                        return;

                    case StackPolicy.Refresh:
                        existing.RefreshDuration();
                        return;

                    case StackPolicy.Stack:
                        existing.RefreshDuration();
                        existing.AddStack(1);
                        return;

                    case StackPolicy.Independent:
                        break; 
                }
            }

            EntityEffect newEffect = new EntityEffect(effectData, _entity, source);
            _effects.Add(newEffect);

            if (effectData is IDurationEffect durationEffect)
                durationEffect.ActiveEffect(_entity);
        }
        
        private void RemoveEffectInternal(EntityEffect entityEffect)
        {
            if (entityEffect.EffectData is IDurationEffect durationEffect)
                durationEffect.UnActiveEffect(_entity);   

            _effects.Remove(entityEffect);
        }

        public void RemoveEffect(AbstractEffectDataSO effectData)
        {
            EntityEffect existing = FindEffect(effectData);
            if (existing != null)
                RemoveEffectInternal(existing);
        }
        
        // 공명 폭발, 슬로우 코드, 화염 비트 등 "상태 가진 적인지" 검사용

        public bool HasEffect(AbstractEffectDataSO effectData)
        {
            return FindEffect(effectData) != null;
        }

        public bool HasEffectById(string effectId)
        {
            return FindEffectById(effectId) != null;
        }

        public EntityEffect GetEffect(AbstractEffectDataSO effectData)
        {
            return FindEffect(effectData);
        }

        public int GetStackCount(AbstractEffectDataSO effectData)
        {
            EntityEffect e = FindEffect(effectData);
            return e?.CurrentStack ?? 0;
        }

        public bool IsFullStack(AbstractEffectDataSO effectData)
        {
            EntityEffect e = FindEffect(effectData);
            return e != null && e.IsFullStack;
        }
        
        
        private EntityEffect FindEffect(AbstractEffectDataSO data)
        {
            // effectId가 있으면 id 비교, 없으면 SO 참조 비교
            bool useId = !string.IsNullOrEmpty(data.effectId);

            for (int i = 0; i < _effects.Count; i++)
            {
                AbstractEffectDataSO d = _effects[i].EffectData;
                if (useId)
                {
                    if (d.effectId == data.effectId) return _effects[i];
                }
                else
                {
                    if (d == data) return _effects[i];
                }
            }
            return null;
        }

        private EntityEffect FindEffectById(string effectId)
        {
            for (int i = 0; i < _effects.Count; i++)
                if (_effects[i].EffectData.effectId == effectId) return _effects[i];
            return null;
        }
    }
}