using System;
using System.Collections.Generic;
using System.Linq;
using Code.Entities;
using UnityEngine;

namespace Code.SkillSystem
{
    public class SkillCompo : MonoBehaviour, IEntityComponent
    {
        public Skill activeSkill; //현재 활성화되어 있는 스킬
        public ContactFilter2D whatIsEnemy;
        public Collider2D[] colliders;

        [SerializeField] private int maxCheckEnemy;

        private Entity _entity;

        private Dictionary<Type, Skill> _skills;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            colliders = new Collider2D[maxCheckEnemy];
            
            _skills = new Dictionary<Type, Skill>();
            GetComponentsInChildren<Skill>().ToList().ForEach(skill => _skills.Add(skill.GetType(), skill));
            _skills.Values.ToList().ForEach(skill => skill.InitializeSkill(_entity, this));
        }

        public T GetSkill<T>() where T : Skill
        {
            Type type = typeof(T);
            return _skills.GetValueOrDefault(type) as T;
        }

        public virtual int GetEnemiesInRange(Transform checkTransform, float range)
            => Physics2D.OverlapCircle(checkTransform.position, range, whatIsEnemy, colliders);
        
        public virtual int GetEnemiesInRange(Vector3 checkPosition, float range)
            => Physics2D.OverlapCircle(checkPosition, range, whatIsEnemy, colliders);

        public virtual Transform FindClosestEnemy(Vector3 checkPosition, float range)
        {
            Transform closestOne = null;
            int cnt = Physics2D.OverlapCircle(checkPosition, range, whatIsEnemy, colliders);
            
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < cnt; i++)
            {
                if (colliders[i].TryGetComponent(out Entity enemy))
                {
                    if (enemy.IsDead) continue;
                    float distanceToEnemy = Vector2.Distance(checkPosition, colliders[i].transform.position);

                    if (distanceToEnemy < closestDistance)
                    {
                        closestDistance = distanceToEnemy;
                        closestOne = colliders[i].transform;
                    }
                }
            }
            return closestOne;
        }
    }
}