using System;
using System.Collections.Generic;
using System.Linq;
using ChipmunkKingdoms.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Work.CHUH._01Scripts.Entities
{
    public abstract class Entity : MonoBehaviour, IContainerComponent
    {
        public bool IsDead { get; set; }
        public UnityEvent OnHitEvent;
        public UnityEvent OnDeadEvent;
        

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            
        }
        
        public ComponentContainer ComponentContainer { get; set; }
        public virtual void OnInitialize(ComponentContainer componentContainer)
        {
            
        }

    }
}