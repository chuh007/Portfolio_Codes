using System;
using UnityEngine;
using Work.CHUH._01Scripts.Entities;

namespace Work.CHUH._01Scripts.Combat
{
    public abstract class Projectile : MonoBehaviour
    {
        protected float _damage;
        protected float _speed;
        protected float _lifeTime;
        protected Entity _owner;
        protected LayerMask _whatIsTarget;
        
        public abstract void InitAndFire(Vector2 dir, float damage, float speed, Entity owner, LayerMask whatIsTarget);
        
    }
}