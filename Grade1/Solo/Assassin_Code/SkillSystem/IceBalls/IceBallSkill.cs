using System;
using System.Collections;
using Code.Core.EventSystems;
using Code.Core.StatSystem;
using Code.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.SkillSystem.IceBalls
{
    public class IceBallSkill : Skill, IReleasable
    {
        [Header("References")] 
        public ParticleSystem chargingEffect;
        public IceBall iceballPrefab;
        [field: SerializeField] public GameEventChannelSO PlayerChannel { get; private set; }
        [field: SerializeField] public StatSO MagicDamageStat { get; private set; }

        [Header("Settings")] 
        public bool isGuidedFire;
        public float guideForce = 25f;
        public float guideRange = 12f;
        public int maxMissiles = 2;
        public float chargeTime = 1f;
        public float damageMultiplier = 1f;
        // public float chargingDuration; 

        private int _currentChargeCount = 0;
        private float _currentChargingTime = 0;
        private bool _isCharging = false;
        private float _currentMagicStatValue;

        public override void InitializeSkill(Entity entity, SkillCompo skillCompo)
        {
            base.InitializeSkill(entity, skillCompo);
            entity.GetCompo<EntityStat>().GetStat(MagicDamageStat).OnValueChange += HandleMagicStatChange;
            _currentMagicStatValue = entity.GetCompo<EntityStat>().GetStat(MagicDamageStat).Value;
        }

        private void OnDestroy()
        {
            _entity.GetCompo<EntityStat>().GetStat(MagicDamageStat).OnValueChange -= HandleMagicStatChange;   
        }

        public override bool AttemptUseSkill()
        {
            if (_cooldownTimer <= 0 && skillEnabled && _isCharging == false)
            {
                UseSkill();
                return true;
            }
            return false; //그렇지 않으면 안쓴다.
        }


        private void HandleMagicStatChange(StatSO stat, float current, float previous)
            => _currentMagicStatValue = current;

        public override void UseSkill()
        {
            //부모의 UseSkill을 사용하지 않는다.
            //chargingEffect.transform.position = GetEffectPosition();
            chargingEffect.Play();
            _currentChargeCount = 0;
            _currentChargingTime = 0;
            _isCharging = true; //차징 시작
        }

        protected override void Update()
        {
            base.Update();

            if (_isCharging == false || _currentChargeCount >= maxMissiles) return;
            
            _currentChargingTime += Time.deltaTime;
            if (_currentChargingTime >= chargeTime)
            {
                _currentChargeCount = Mathf.Clamp(_currentChargeCount + 1, 0, maxMissiles);
                _currentChargingTime = 0;
                SendChargedEvent();
            }
        }
        
        //한개 충전 될때마다 발생하는 이벤트
        private void SendChargedEvent()
        {
            SkillChargedEvent chargedEvent = PlayerEvents.SkillChargedEvent;
            chargedEvent.chargeCount = _currentChargeCount;
            PlayerChannel.RaiseEvent(chargedEvent);
        }
        
        /// <summary>
        /// 키보드에서 손을 떼었을 때 차징된 갯수만큼 발사되도록 하는 매서드
        /// </summary>
        public void ReleaseSkill()
        {
            if (_isCharging == false) return;
            
            _isCharging = false;
            chargingEffect.Stop();
            
            //여기서 차징된 갯수가 0보다 크다면 발사로직을 시작해야하고, 그렇지 않다면 바로 종료하면 된다.
            if (_currentChargeCount > 0)
            {
                StartCoroutine(ShootingIceBalls());
                _cooldownTimer = cooldown; //실질적 발사가 있었을 때 쿨 다운 돌아간다.
            }
            else
            {
                PlayerChannel.RaiseEvent(PlayerEvents.SkillChargeEndEvent);
            }
        }

        private IEnumerator ShootingIceBalls()
        {
            WaitForSeconds wait = new WaitForSeconds(0.3f);
            Vector3 originalPosition = chargingEffect.transform.position;
            EntityMover mover = _entity.GetCompo<EntityMover>();

            mover.CanManualMove = false; //못움직이게 막아주고

            //유도 거리내에 적이 있다면
            int enemyCount = _skillCompo.GetEnemiesInRange(originalPosition, guideRange);
            
            for (int i = 0; i < _currentChargeCount; i++)
            {
                IceBall iceBall = Instantiate(iceballPrefab);
                Vector3 generatePosition = originalPosition + (Vector3)Random.insideUnitCircle * 0.3f;
                
                //이건 나중에 EntityDamageCompo 나오면 다 바꾼다.
                float damage = _currentMagicStatValue * damageMultiplier;

                if (isGuidedFire && enemyCount > 0) //유도탄이고 적도 존재한다.
                {
                    int index = Random.Range(0, enemyCount);
                    iceBall.guideTargetTrm = _skillCompo.colliders[index].transform; //오버랩을 써서 가져온 녀석중에 하나
                }
                iceBall.SetUpAndFire(this, generatePosition, _entity.transform.right, damage, _entity);
                mover.AddForceToEntity(_entity.transform.right * -1f);
                yield return wait;
            }
            mover.CanManualMove = true;
            
            PlayerChannel.RaiseEvent(PlayerEvents.SkillChargeEndEvent);
        }
    }
}