using UnityEngine;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Bus;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;

namespace _Code.LCH._02.Scripts.Level
{
    public class ExperienceManager : MonoBehaviour
    {
        public static ExperienceManager Instance { get; private set; }
        
        [SerializeField] private float collectRadius   = 3f;
        [SerializeField] private float attractDuration = 0.5f;
        [SerializeField] private int   maxActiveOrbs   = 400;
        [SerializeField] private float redOrbExpAmount = 100f;
        [SerializeField] private float greenOrbRatio = 0.25f;
        
        private Transform         _playerTransform;
        private readonly List<ExpOrb> _activeOrbs = new();
        
        public float CollectRadius
        {
            get => collectRadius;
            set => collectRadius = value;
        }

        public float BaseCollectRadius => collectRadius;
        
        public float AttractDuration => attractDuration;
        public Transform PlayerTransform => _playerTransform;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
        
        public void SetPlayer(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }
        
        public void RegisterOrb(ExpOrb orb)
        {
            if (!_activeOrbs.Contains(orb))
                _activeOrbs.Add(orb);
        }
        
        public void UnregisterOrb(ExpOrb orb)
        {
            _activeOrbs.Remove(orb);
        }

        public void SpawnOrMergeOrb(
            PoolManagerSO poolManager,
            PoolItemSO poolItem,
            ExpOrbDataSO data,
            Vector3 position)
        {
            if (data == null) return;

            float expAmount = data.expAmount;
            if (expAmount <= 0f) return;

            CompactActiveOrbs();
            int activeLimit = Mathf.Max(1, maxActiveOrbs);

            if (_activeOrbs.Count >= activeLimit)
                expAmount += RecycleOverflowOrb();

            if (_activeOrbs.Count < activeLimit)
            {
                if (SpawnOrb(poolManager, poolItem, data, position, expAmount))
                    return;
            }

            GrantExperienceDirectly(expAmount);
        }

        public void AttractAllOrbs()
        {
            AttractAllOrbs(attractDuration);
        }

        public void AttractAllOrbs(float duration)
        {
            if (_playerTransform == null) return;

            for (int i = _activeOrbs.Count - 1; i >= 0; i--)
            {
                var orb = _activeOrbs[i];
                if (orb == null)
                {
                    _activeOrbs.RemoveAt(i);
                    continue;
                }

                orb.StartAttract(_playerTransform, duration);
            }
        }
        
        private void Update()
        {
            if (_playerTransform == null) return;
            
            float   sqrRadius = collectRadius * collectRadius;
            Vector3 playerPos = _playerTransform.position;
            
            for (int i = _activeOrbs.Count - 1; i >= 0; i--)
            {
                var orb = _activeOrbs[i];
                if (orb == null)
                {
                    _activeOrbs.RemoveAt(i);
                    continue;
                }
                
                if (orb.IsAttracting) continue;
                
                if ((orb.transform.position - playerPos).sqrMagnitude <= sqrRadius)
                    orb.StartAttract(_playerTransform);
            }

            CompactActiveOrbs();
        }

        private bool SpawnOrb(
            PoolManagerSO poolManager,
            PoolItemSO poolItem,
            ExpOrbDataSO data,
            Vector3 position,
            float expAmount)
        {
            if (poolManager == null) return false;

            PoolItemSO item = poolItem != null ? poolItem : data.poolItem;
            if (item == null) return false;

            if (poolManager.Pop(item) is not ExpOrb orb) return false;

            orb.transform.position = position;
            orb.Init(data, expAmount);
            RefreshOrbVisual(orb);
            return true;
        }

        private void GrantExperienceDirectly(float expAmount)
        {
            Bus<ExpGainEvent>.Raise(new ExpGainEvent(expAmount));
        }

        private float RecycleOverflowOrb()
        {
            ExpOrb recycledOrb = FindRecycleSource(false);
            if (recycledOrb == null)
                recycledOrb = FindRecycleSource(true);
            if (recycledOrb == null)
                return 0f;

            float recycledExp = recycledOrb.ExpAmount;
            recycledOrb.ReturnToPool();

            return recycledExp;
        }

        private ExpOrb FindRecycleSource(bool includeAttracting)
        {
            ExpOrb recycleSource = null;
            float farthestSqrDistance = -1f;
            Vector3 playerPos = _playerTransform != null ? _playerTransform.position : Vector3.zero;

            for (int i = _activeOrbs.Count - 1; i >= 0; i--)
            {
                ExpOrb orb = _activeOrbs[i];
                if (!IsValidOrb(orb))
                {
                    _activeOrbs.RemoveAt(i);
                    continue;
                }

                if (!includeAttracting && orb.IsAttracting) continue;

                float sqrDistance = _playerTransform != null
                    ? (orb.transform.position - playerPos).sqrMagnitude
                    : i;
                if (sqrDistance <= farthestSqrDistance) continue;

                farthestSqrDistance = sqrDistance;
                recycleSource = orb;
            }

            return recycleSource;
        }

        private void RefreshOrbVisual(ExpOrb orb)
        {
            if (orb == null) return;
            orb.RefreshStackVisual(GreenOrbExpAmount, RedOrbExpAmount);
        }

        private float RedOrbExpAmount => Mathf.Max(0.01f, redOrbExpAmount);
        private float GreenOrbExpAmount => Mathf.Max(0.01f, RedOrbExpAmount * Mathf.Clamp01(greenOrbRatio));

        private void CompactActiveOrbs()
        {
            for (int i = _activeOrbs.Count - 1; i >= 0; i--)
            {
                if (IsValidOrb(_activeOrbs[i])) continue;

                _activeOrbs.RemoveAt(i);
            }
        }

        private bool IsValidOrb(ExpOrb orb)
        {
            return orb != null && orb.gameObject.activeInHierarchy;
        }
    }
}
