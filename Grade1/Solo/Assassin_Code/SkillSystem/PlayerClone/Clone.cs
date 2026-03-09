using System.Collections;
using System.Collections.Generic;
using Code.Animators;
using Code.Combats;
using Code.Core.StatSystem;
using Code.Entities;
using DG.Tweening;
using UnityEngine;

namespace Code.SkillSystem.PlayerClone
{
    public class Clone : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private DamageCaster damageCaster;
        [SerializeField] private AnimParamSO comboCounterParam;
        [SerializeField] private StatSO damageStat; //이 스킬의 데미지를 주관할 주 스탯

        [SerializeField] private List<AttackDataSO> skillAttackDataList;

        private Transform _currentClosestEnemy;
        private float _facingDirection = 1f;

        private CloneSkill _skill;
        private int _comboCounter;
        private Entity _skillOwner;
        private int _duplicateCount;

        public void SetUpClone(Entity owner, CloneSkill skill, Transform targetTrm, int duplicateCount)
        {
            _skillOwner = owner;
            _comboCounter = Random.Range(0, skillAttackDataList.Count);
            animator.SetInteger(comboCounterParam.hashValue, _comboCounter);

            _skill = skill;
            damageCaster.InitCaster(owner);

            _duplicateCount = duplicateCount;
            
            FacingToClosestTarget(targetTrm);
            StartCoroutine(FadeAfterDelay(skill.cloneDuration));
        }
        
        private void FacingToClosestTarget(Transform targetTrm)
        {
            if(targetTrm == null) return;

            if (transform.position.x > targetTrm.position.x)
            {
                _facingDirection = -1f;
                transform.Rotate(0, 180f, 0);
            }
        }
        
        private IEnumerator FadeAfterDelay(float skillCloneDuration)
        {
            yield return new WaitForSeconds(skillCloneDuration);
            spriteRenderer.DOFade(0, 0.7f).OnComplete(()=> Destroy(gameObject));
        }

        public void AttackTrigger()
        {
            AttackDataSO attackData = skillAttackDataList[_comboCounter];
            float statValue = _skillOwner.GetCompo<EntityStat>().GetStat(damageStat).Value;

            //스킬에 Calcuate데이터 만들어 하는식으로 변경할꺼다.
            float finalDamage = statValue * attackData.damageMultiplier * _skill.damageMultiplier
                                + attackData.damageIncrease;

            bool success = damageCaster.CastDamage(finalDamage, attackData.knockBackForce, attackData.isPowerAttack);
            
            if (success && _skill.canDuplicateClone && _duplicateCount < _skill.maxDuplicateCounter) //공격성공했고 복제가 언락되었다면
            {
                //Random.value => 0~1까지 랜덤 값 반환
                if (Random.value < _skill.duplicatePercent * Mathf.Pow(_skill.reducePercentByCount, _duplicateCount))
                {
                    _skill.CreateClone(transform, new Vector3(1.5f * _facingDirection, 0), _duplicateCount + 1);
                }
            }
        }
    }
}