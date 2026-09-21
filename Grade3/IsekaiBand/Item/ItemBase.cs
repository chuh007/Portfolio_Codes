using _Code.LCH._02.Scripts.Level;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Item
{
    public abstract class ItemBase : MonoBehaviour, IPoolable
    {
        [SerializeField] private PoolItemSO poolItem;
        [SerializeField] private LayerMask collectorLayer = (1 << 6) | (1 << 11);
        
        private Pool _pool;
        private Transform _target;
        private bool _isAttracting;
        private bool _isCollected;
        private float _remainingTime;
        
        public PoolItemSO PoolItem => poolItem;
        
        public virtual void ResetItem()
        {
            _target = null;
            _isAttracting = false;
            _isCollected = false;
            _remainingTime = 0f;
        }
        
        public void SetUpPool(Pool pool)
        {
            _pool = pool;
        }
        
        public void StartAttract(Transform target, float duration)
        {
            if (_isCollected) return;

            _target = target;
            _isAttracting = true;
            _remainingTime = Mathf.Max(0.01f, duration);
        }
        
        protected abstract void OnCollect();
        
        protected virtual void Update()
        {
            if (_isCollected) return;
            if (_isAttracting)
            {
                UpdateAttract();
                return;
            }
            
            TryStartAttractByDistance();
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCollected) return;
            if ((collectorLayer.value & (1 << other.gameObject.layer)) == 0) return;
            Collect();
        }
        
        private void TryStartAttractByDistance()
        {
            var manager = ExperienceManager.Instance;
            if (manager == null || manager.PlayerTransform == null) return;

            float sqrRadius = manager.CollectRadius * manager.CollectRadius;
            if ((transform.position - manager.PlayerTransform.position).sqrMagnitude <= sqrRadius)
                StartAttract(manager.PlayerTransform, manager.AttractDuration);
        }

        private void UpdateAttract()
        {
            if (_target == null)
            {
                _isAttracting = false;
                return;
            }

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
            if (_isCollected) return;
            
            _isCollected = true;
            OnCollect();
            
            if (_pool != null)
                _pool.Push(this);
            else
                gameObject.SetActive(false);
        }
    }
}
