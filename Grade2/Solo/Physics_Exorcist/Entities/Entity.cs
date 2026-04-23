using System;
using System.Collections.Generic;
using System.Linq;
using _01Scripts.Combat;
using _01Scripts.Core.EventSystem;
using _01Scripts.Players;
using UnityEngine;
using UnityEngine.Events;

namespace _01Scripts.Entities
{
    public abstract class Entity : MonoBehaviour, IDamageable
    {
        [field:SerializeField] public GameEventChannelSO TurnChannel { get; private set; }
        public delegate void OnDamageHandler(DamageData damage, Entity dealer);
        public event OnDamageHandler OnDamage;

        public UnityEvent OnDefense;
        public UnityEvent OnHit;
        public UnityEvent<Entity> OnDead;

        public bool IsDead { get; set; }
        public bool IsDefense { get; set; }

        protected Dictionary<Type, IEntityComponent> _components;

        protected virtual void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponents();
            InitializeComponents();
            AfterInitialize();
        }

        
        
        private void AddComponents()
        {
            GetComponentsInChildren<IEntityComponent>().ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        }

        private void InitializeComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        protected virtual void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitialize>().ToList().ForEach(compo => compo.AfterInitialize());
            OnHit.AddListener(HandleHit);
            OnDead.AddListener(HandleDead);
        }
        
        protected virtual void OnDestroy()
        {
            OnHit.RemoveListener(HandleHit);
            OnDead.RemoveListener(HandleDead);
        }
        
        protected abstract void HandleHit();
        protected abstract void HandleDead(Entity entity);
        
        public T GetCompo<T>(bool isDerived = false) where T : IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out IEntityComponent component))
                return (T)component;
            
            if(isDerived == false) return default(T);
            
            Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(T)));
            if(findType != null) 
                return (T)_components[findType];
            
            return default(T);
        }

        public void ApplyDamage(DamageData damage, Entity dealer)
            => OnDamage?.Invoke(damage, dealer);

        public bool TryCastDamage()
        {
            if (IsDefense)
            {
                OnDefense?.Invoke();
                return false;
            }
            return true;
        }
    }
}

