using Code.Animators;
using UnityEngine;

namespace Code.Entities.FSM
{
    public abstract class EntityState
    {
        protected Entity _entity;

        protected AnimParamSO _animParam;
        protected bool _isTriggerCall;

        protected EntityRenderer _renderer;

        public EntityState(Entity entity, AnimParamSO animParam)
        {
            _entity = entity;
            _animParam = animParam;
            _renderer = _entity.GetCompo<EntityRenderer>(true);
        }

        public virtual void Enter()
        {
            _renderer.SetParam(_animParam, true);
            _isTriggerCall = false;
        }

        public virtual void Update()
        {
            
        }

        public virtual void Exit()
        {
            _renderer.SetParam(_animParam, false);
        }

        public virtual void AnimationEndTrigger() => _isTriggerCall = true;
    }
}