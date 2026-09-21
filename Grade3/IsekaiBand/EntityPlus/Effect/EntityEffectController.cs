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
        private readonly List<EntityEffect> _effects = new();
        private bool _isClearing;

        public Entity Owner => _entity;
        public IReadOnlyList<EntityEffect> ActiveEffects => _effects;
        
        public void Initialize(Entity entity)
        {
            if (_entity != null)
                _entity.OnDead.RemoveListener(HandleDeath);
            ClearEffects();
            _entity = entity;
            _entity.OnDead.AddListener(HandleDeath);
        }

        private void Update()
        {
            int i = 0;
            while (i < _effects.Count)
            {
                EntityEffect effect = _effects[i];
                effect.UpdateEffect();
                // 틱 피해로 사망하면 콜백 안에서 목록이 비워질 수 있다.
                if (i >= _effects.Count || _effects[i] != effect) continue;
                if (effect.IsExpired)
                    RemoveEffectInternal(effect);
                else
                    i++;
            }
        }

        public void AddEffect(AbstractEffectDataSO effectData)
        {
            AddEffect(effectData, null);
        }

        public void AddEffect(AbstractEffectDataSO effectData, Entity source)
        {
            if (effectData == null || _entity == null || _entity.IsDead || _isClearing) return;
            AddEffect(effectData.CreateEffect(_entity, source));
        }

        public void AddEffect(EntityEffect effect)
        {
            if (effect == null || effect.EffectTarget != _entity || _entity == null
                || _entity.IsDead || _isClearing || effect.IsExpired) return;

            AbstractEffectDataSO effectData = effect.EffectData;
            EntityEffect existing = FindEffect(effectData);

            if (existing != null)
            {
                switch (effectData.stackPolicy)
                {
                    case StackPolicy.Ignore:
                        return;

                    case StackPolicy.Refresh:
                    case StackPolicy.Stack:
                        existing.RefreshFrom(effect);
                        return;

                    case StackPolicy.Independent:
                        break; 
                }
            }

            _effects.Add(effect);
            effect.Activate();
        }
        
        private void RemoveEffectInternal(EntityEffect entityEffect)
        {
            if (_effects.Remove(entityEffect))
                entityEffect.Deactivate();
        }

        public void ClearEffects(bool targetDied = false)
        {
            if (_isClearing) return;
            _isClearing = true;
            using var lease = UnityEngine.Pool.ListPool<EntityEffect>.Get(out var effects);
            effects.AddRange(_effects);
            _effects.Clear();
            try
            {
                foreach (EntityEffect effect in effects)
                {
                    if (targetDied && !effect.IsExpired)
                        effect.OnTargetDeath();
                    effect.Deactivate();
                }
            }
            finally
            {
                _isClearing = false;
            }
        }

        private void HandleDeath() => ClearEffects(true);

        private void OnDisable() => ClearEffects(_entity != null && _entity.IsDead);

        private void OnDestroy()
        {
            if (_entity != null)
                _entity.OnDead.RemoveListener(HandleDeath);
            ClearEffects();
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
            if (data == null) return null;
            for (int i = 0; i < _effects.Count; i++)
            {
                if (!_effects[i].IsExpired && _effects[i].Matches(data)) return _effects[i];
            }
            return null;
        }

        private EntityEffect FindEffectById(string effectId)
        {
            for (int i = 0; i < _effects.Count; i++)
                if (!_effects[i].IsExpired && _effects[i].EffectData.effectId == effectId) return _effects[i];
            return null;
        }
    }
}
