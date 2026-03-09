using Code.Core.EventSystems;
using Code.Core.Pool;
using Code.Entities;
using Unity.Behavior;
using UnityEngine;

namespace Code.Enemies
{
    public abstract class BTEnemy : Entity
    {
        [field: SerializeField] public EntityFinderSO PlayerFinder { get; protected set; }
        public LayerMask whatIsPlayer;
        public bool isPlayerFound = false;

        public float FacingDirection => _renderer.FacingDirection;

        //임시코드
        [SerializeField] protected GameEventChannelSO playerChannel;
        [SerializeField] protected int dropExp = 10;
        
        protected BehaviorGraphAgent _btAgent;
        protected EntityRenderer _renderer;
        protected EnemyFOV _fOV;

        protected override void AddComponentToDictionary()
        {
            base.AddComponentToDictionary();
            EnemyCounter.Instance.EnemyCountValueChange(1);
            _btAgent = GetComponent<BehaviorGraphAgent>();
            Debug.Assert(_btAgent != null, $"{gameObject.name} does not have an BehaviorGraphAgent");
            
            _renderer = GetCompo<EntityRenderer>();
            Debug.Assert(_renderer != null, $"{gameObject.name} does not have an EntityRenderer");

            _fOV = GetCompo<EnemyFOV>();
            Debug.Assert(_fOV != null, $"{gameObject.name} does not have an FOV");

            _fOV.viewAngle = GetBlackboardVariable<float>("Angle");
            _fOV.viewDistance = GetBlackboardVariable<float>("DetectRange");
            _fOV.DrawFOV();
        }

        protected virtual void Start()
        {
            //do nothing
        }

        protected override void HandleDead()
        {
            ParticleManager.Instance.ParticlePlay(PollingType.EnemyDeadParticle, transform.position);
            _fOV.gameObject.SetActive(false);
        }
        public BlackboardVariable<T> GetBlackboardVariable<T>(string key)
        {
            if (_btAgent.GetVariable(key, out BlackboardVariable<T> result))
            {
                return result;
            }
            return default;
        }
        protected override void OnDestroy()
        {
            if(EnemyCounter.Instance != null)
                EnemyCounter.Instance.EnemyCountValueChange(-1);
            base.OnDestroy();
        }

    }
}