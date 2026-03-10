using Code.Core.EventSystems;
using Code.Entities;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.SkillSystem.PlayerClone
{
    public class CloneSkill : Skill
    {
        [Header("Event channel")] 
        [field: SerializeField] public GameEventChannelSO PlayerChannel { get; private set; }
        
        [Header("Clone info")] 
        [SerializeField] private Clone _clonePrefab;

        public float cloneDuration;
        public bool createCloneOnDashStart;
        public bool createCloneOnDashEnd;
        public bool createCloneOnCounterAttack;
        public float findEnemyRadius = 8f;
        public float damageMultiplier = 1f; //증뎀량
        public float generateProbability = 0.1f;

        [SerializeField] private float counterCloneOffset = 2f;
        [SerializeField] private float delayCreateCounterClone = 0.4f;
        
        [Header("Duplicate Clone")] 
        public bool canDuplicateClone;
        public float duplicatePercent;
        public int maxDuplicateCounter = 3;
        public float reducePercentByCount = 0.5f;
        //복제 클론은 복제가 될 수록 확률이 1/2로 떨어지고 최대 3개까지만 복제되도록 한다. 

        private EntityRenderer _renderer;

        public override void InitializeSkill(Entity entity, SkillCompo skillCompo)
        {
            base.InitializeSkill(entity, skillCompo);
            _renderer = entity.GetCompo<EntityRenderer>();
            PlayerChannel.AddListener<DashStartEvent>(CreateCloneOnDashStart);
            PlayerChannel.AddListener<DashEndEvent>(CreateCloneOnDashEnd);
            PlayerChannel.AddListener<CounterSuccessEvent>(CreateCloneOnCounterAttack);
            PlayerChannel.AddListener<PlayerAttackSuccess>(CreateCloneOnAttackSuccess);
        }

        private void OnDestroy()
        {
            PlayerChannel.RemoveListener<DashStartEvent>(CreateCloneOnDashStart);
            PlayerChannel.RemoveListener<DashEndEvent>(CreateCloneOnDashEnd);
            PlayerChannel.RemoveListener<CounterSuccessEvent>(CreateCloneOnCounterAttack);
            PlayerChannel.RemoveListener<PlayerAttackSuccess>(CreateCloneOnAttackSuccess);
        }

        //originTrm은 플레이어의 위치다. 
        public void CreateClone(Transform originTrm, Vector3 offset, int duplicateCount = 1)
        {
            Clone newClone = Instantiate(_clonePrefab);
            newClone.transform.position = originTrm.position + offset;
            Transform targetTrm = _skillCompo.FindClosestEnemy(newClone.transform.position, findEnemyRadius);
            newClone.SetUpClone(_entity, this, targetTrm, duplicateCount); //클론을 셋업한다.
        }

        private void CreateCloneOnDashStart(DashStartEvent evt)
        {
            if(createCloneOnDashStart)
                CreateClone(_entity.transform, Vector3.zero);
        }

        private void CreateCloneOnDashEnd(DashEndEvent evt)
        {
            if(createCloneOnDashEnd)
                CreateClone(_entity.transform, Vector3.zero);
        }

        private void CreateCloneOnCounterAttack(CounterSuccessEvent evt)
        {
            if (createCloneOnCounterAttack)
                DOVirtual.DelayedCall(delayCreateCounterClone, () =>
                {
                    float cloneXPosition = counterCloneOffset * _renderer.FacingDirection;
                    CreateClone(evt.target, new Vector3(cloneXPosition, 0));
                });
        }
        
        private void CreateCloneOnAttackSuccess(PlayerAttackSuccess evt)
        {
            if (skillEnabled == false) return;

            if (Random.value < generateProbability)
            {
                float cloneXPosition = 2 * counterCloneOffset * _renderer.FacingDirection;
                CreateClone(_entity.transform, new Vector3(cloneXPosition, 0));
            }
        }
    }
}

