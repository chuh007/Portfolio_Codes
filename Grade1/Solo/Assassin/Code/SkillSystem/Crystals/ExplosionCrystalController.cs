using System;
using Code.Entities;
using UnityEngine;

namespace Code.SkillSystem.Crystals
{
    public class ExplosionCrystalController : CrystalController
    {
        [Header("Prefab References")]
        [SerializeField] private GameObject crystalPrefab;
        
        private Crystal _currentCrystal;
        private float _timer;

        public override void CreateCrystal()
        {
            Vector3 position = _owner.transform.position + _owner.transform.right * 0.8f;
            _currentCrystal = Instantiate(crystalPrefab, position, Quaternion.identity).GetComponent<Crystal>();
            _currentCrystal.SetUp(_damageStatValue, skill, this, _owner);
            _timer = skill.timeOut;
            SetCrystalStatus(true); //크리스탈 소유 상태로 변경
        }

        public override void UseCrystal()
        {
            _currentCrystal.TriggerCrystal();
            _currentCrystal = null;
            SetCrystalStatus(false);
        }

        private void Update()
        {
            if (_currentCrystal != null)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    UseCrystal();
                }
            }
            
        }
    }
}