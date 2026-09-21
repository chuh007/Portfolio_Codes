using _Code.LCH._02.Scripts.Bus;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _Code.LCH._02.Scripts.Level
{
    public class ExpOrb : MonoBehaviour, IPoolable
    {
        private const float PickupSoundSuppressionWindow = 0.02f;

        [SerializeField] private PoolItemSO _poolItem;
        [SerializeField] private Color stackedGreenColor = Color.green;
        [SerializeField] private Color stackedRedColor = Color.red;

        private Pool         _pool;
        private Transform    _target;
        private bool         _isAttracting;
        private bool         _isMerged;
        private float        _expAmount;
        private float        _remainingTime;
        private SpriteRenderer _spriteRenderer;
        private Color          _defaultColor = Color.white;

        public PoolItemSO PoolItem    => _poolItem;
        public bool IsAttracting      => _isAttracting;
        public bool IsMerged          => _isMerged;
        public float ExpAmount        => _expAmount;
        public void SetUpPool(Pool p) => _pool = p;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_spriteRenderer != null)
                _defaultColor = _spriteRenderer.color;
        }

        public void ResetItem()
        {
            ExperienceManager.Instance?.UnregisterOrb(this);
            _target       = null;
            _isAttracting = false;
            _isMerged     = false;
            _expAmount    = 0f;
            SetColor(_defaultColor);
        }

        public void Init(ExpOrbDataSO data)
        {
            Init(data, data != null ? data.expAmount : 0f);
        }

        public void Init(ExpOrbDataSO data, float expAmount)
        {
            _target       = null;
            _isAttracting = false;
            _isMerged     = false;
            _expAmount    = Mathf.Max(0f, expAmount);
            SetColor(_defaultColor);
            ExperienceManager.Instance?.RegisterOrb(this);
        }

        private void OnDisable()
        {
            ExperienceManager.Instance?.UnregisterOrb(this);
        }

        private void OnDestroy()
        {
            ExperienceManager.Instance?.UnregisterOrb(this);
        }

        public void AddExp(float amount)
        {
            _expAmount += Mathf.Max(0f, amount);
        }

        public void SetMerged(bool isMerged)
        {
            _isMerged = isMerged;
            SetColor(isMerged ? Color.red : _defaultColor);
        }

        public void RefreshStackVisual(float greenExpAmount, float redExpAmount)
        {
            float greenThreshold = Mathf.Max(0.01f, greenExpAmount);
            float redThreshold = Mathf.Max(greenThreshold, redExpAmount);

            _isMerged = _expAmount >= greenThreshold;
            if (_expAmount >= redThreshold)
                SetColor(stackedRedColor);
            else if (_expAmount >= greenThreshold)
                SetColor(stackedGreenColor);
            else
                SetColor(_defaultColor);
        }

        public void ReturnToPool()
        {
            ExperienceManager.Instance?.UnregisterOrb(this);
            _pool?.Push(this);
        }

        public void StartAttract(Transform target)
        {
            StartAttract(target, ExperienceManager.Instance != null
                ? ExperienceManager.Instance.AttractDuration
                : 0.5f);
        }

        public void StartAttract(Transform target, float duration)
        {
            _target        = target;
            _isAttracting  = true;
            _remainingTime = GetDistanceScaledAttractDuration(target, duration);
        }

        private float GetDistanceScaledAttractDuration(Transform target, float duration)
        {
            if (target == null)
                return Mathf.Max(0.01f, duration);

            float referenceDistance = ExperienceManager.Instance != null
                ? Mathf.Max(0.01f, ExperienceManager.Instance.CollectRadius)
                : 1f;
            float distance = Vector2.Distance(transform.position, target.position);

            return Mathf.Max(0.01f, duration * distance / referenceDistance);
        }

        private void Update()
        {
            if (!_isAttracting || _target == null) return;

            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0f)
            {
                transform.position = _target.position;
                Collect();
                return;
            }

            transform.position = Vector3.Lerp(
                transform.position,
                _target.position,
                Time.deltaTime / _remainingTime);
        }

        private void Collect()
        {
            Bus<ExpGainEvent>.Raise(new ExpGainEvent(_expAmount));
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.ExperiencePickup,
                SoundType.SFX,
                volumeMultiplier: 2f,
                suppressDuplicateThisFrame: true,
                duplicateSuppressionWindowSeconds: PickupSoundSuppressionWindow));
            ReturnToPool();
        }

        private void SetColor(Color color)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.color = color;
        }
    }
}
