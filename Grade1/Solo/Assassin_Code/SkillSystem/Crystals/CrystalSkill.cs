using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.StatSystem;
using Code.Entities;
using UnityEngine;

namespace Code.SkillSystem.Crystals
{
    public class CrystalSkill : Skill
    {
        public enum CrystalType
        {
            TriggerExplosion, Multiple
        }

        [Header("Default settings")] 
        [SerializeField] private StatSO damageStat;
        public float timeOut = 5f;
        public float damageMultiplier = 1f;

        [Header("Crystal type")] 
        public CrystalType crystalType;

        private Dictionary<CrystalType, CrystalController> _controllers;
        private CrystalController _currentController;

        private float _damageStatValue;
        
        public override void InitializeSkill(Entity entity, SkillCompo skillCompo)
        {
            base.InitializeSkill(entity, skillCompo);
            StatSO stat = entity.GetCompo<EntityStat>().GetStat(damageStat);
            stat.OnValueChange += HandleDamageStatChange;
            _damageStatValue = stat.Value;
            
            _controllers = new Dictionary<CrystalType, CrystalController>();
            GetComponentsInChildren<CrystalController>().ToList()
                .ForEach(controller => _controllers.Add(controller.crystalType, controller));
            Debug.Assert(_controllers.Count > 0, "No CrystalController found");

            SetCurrentController(crystalType);
        }
        
        private void OnDestroy()
        {
            _entity.GetCompo<EntityStat>().GetStat(damageStat).OnValueChange -= HandleDamageStatChange;
        }

        private void SetCurrentController(CrystalType newType)
        {
            if (_currentController != null)
                _currentController.OnCrystalStatusChange -= HandleCrystalStatusChange;

            crystalType = newType;
            _currentController = _controllers[newType];
            _currentController.SetUp(this, _entity);
            _currentController.OnCrystalStatusChange += HandleCrystalStatusChange;
            _currentController.SetDamageStatValue(_damageStatValue);
        }

        private void HandleCrystalStatusChange(bool before, bool next)
        {
            if (before && next == false)  //이전에 크리스탈을 가지고 있다가 이번에 없어진거
            {
                _cooldownTimer = cooldown; //쿨타임 시작.
            }
        }
        
        private void HandleDamageStatChange(StatSO stat, float current, float previous)
        {
            _damageStatValue = current;
            _currentController.SetDamageStatValue(_damageStatValue); //여기 추가되었습니다.
        }

        public override bool AttemptUseSkill()
        {
            if (skillEnabled == false) return false;
            if (_currentController == null) return false;
            //쿨타임이 돌고 있는 상태.
            if (_cooldownTimer > 0 && _currentController.HasCrystal == false) return false;
            UseSkill();
            return true;
        }

        public override void UseSkill()
        {
            if(_currentController.HasCrystal == false)
                _currentController.CreateCrystal();
            else
            {
                Debug.Log("use");
                _currentController.UseCrystal();
            }
        }
        
        public Transform FindClosestTarget(Transform origin, float radius)
            => _skillCompo.FindClosestEnemy(origin.position, radius);
        
    }
}