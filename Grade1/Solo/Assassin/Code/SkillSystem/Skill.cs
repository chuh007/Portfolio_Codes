using System;
using Code.Entities;
using UnityEngine;

namespace Code.SkillSystem
{
    public delegate void CooldownInfo(float current, float totalTime);
    
    public abstract class Skill : MonoBehaviour
    {
        public bool skillEnabled = false;
        
        [SerializeField] protected float cooldown;
        protected float _cooldownTimer;
        protected Entity _entity;
        protected SkillCompo _skillCompo;

        public bool IsCooldown => _cooldownTimer > 0f;
        public event CooldownInfo OnCooldown;

        public virtual void InitializeSkill(Entity entity, SkillCompo skillCompo)
        {
            _entity = entity;
            _skillCompo = skillCompo;
        }

        protected virtual void Update()
        {
            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;

                if (_cooldownTimer <= 0)
                    _cooldownTimer = 0;
                
                OnCooldown?.Invoke(_cooldownTimer, cooldown);
            }
        }

        public virtual bool AttemptUseSkill()
        {
            if (_cooldownTimer <= 0 && skillEnabled)
            {
                _cooldownTimer = cooldown;
                UseSkill();
                return true;
            }
            return false;
        }

        public virtual void UseSkill()
        {
            //여기서 나중에 스킬을 썼음을 알려주는 피드백이 필요하다.
        }

        public virtual void UseSkillWithoutCooltimeAndEffect()
        {
            //자동발동 스킬들이 이용하기 위해 만든 함수.
        }
    }
}