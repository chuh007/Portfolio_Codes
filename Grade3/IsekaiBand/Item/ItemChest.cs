using System;
using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Combat;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Item
{
    [Serializable]
    public struct WeightedPoolItem
    {
        [SerializeField] private PoolItemSO item;
        [SerializeField] private int weight;

        public PoolItemSO Item => item;
        public int Weight => weight;
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class ItemChest : MonoBehaviour, IDamageable, IHealth, IPoolable
    {
        [SerializeField] private PoolItemSO poolItem;
        [SerializeField] private float maxHealth = 1f;

        private Pool _pool;
        private PoolManagerSO _poolManager;
        private IReadOnlyList<WeightedPoolItem> _dropTable;
        private Action<ItemChest> _onOpened;
        private float _currentHealth;

        public PoolItemSO PoolItem => poolItem;
        public float MaxHealth => maxHealth;
        public float CurrentHealth => _currentHealth;
        public Action<float> OnHpChanged { get; set; }
        public bool IsOpened { get; private set; }

        private void Awake()
        {
            var body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
        }

        public void Initialize(
            PoolManagerSO poolManager,
            IReadOnlyList<WeightedPoolItem> dropTable,
            Action<ItemChest> onOpened)
        {
            _poolManager = poolManager;
            _dropTable = dropTable;
            _onOpened = onOpened;
        }

        public void TakeDamage(DamageData damage, Vector2 direction = default, Entity dealer = null)
        {
            TakeDamage(damage.Damage, damage.IgnoreDefense, damage.DamageType, damage.IsFixedDamage);
        }

        public void TakeDamage(float damage, bool ignoreDefense = false, DamageType damageType = DamageType.Physical, bool isFixedDamage = false)
        {
            if (IsOpened) return;

            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, maxHealth);
            OnHpChanged?.Invoke(_currentHealth);

            if (_currentHealth <= 0f)
                Open();
        }

        public void TakeHeal(float heal)
        {
            if (IsOpened) return;

            _currentHealth = Mathf.Clamp(_currentHealth + heal, 0f, maxHealth);
            OnHpChanged?.Invoke(_currentHealth);
        }

        public void ResetItem()
        {
            IsOpened = false;
            _currentHealth = maxHealth;
            OnHpChanged?.Invoke(_currentHealth);
        }

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
        }

        private void Open()
        {
            if (IsOpened) return;

            IsOpened = true;
            DropItem();
            _onOpened?.Invoke(this);

            if (_pool != null)
                _pool.Push(this);
            else
                gameObject.SetActive(false);
        }

        private void DropItem()
        {
            PoolItemSO item = PickDropItem();
            if (item == null || _poolManager == null) return;

            IPoolable drop = _poolManager.Pop(item);
            if (drop == null)
            {
                Debug.LogWarning($"[ItemChest] {item.name} is not registered in PoolManager.");
                return;
            }

            drop.gameObject.transform.position = transform.position;
        }

        private PoolItemSO PickDropItem()
        {
            if (_dropTable == null || _dropTable.Count == 0) return null;

            int totalWeight = 0;
            for (int i = 0; i < _dropTable.Count; i++)
                totalWeight += Mathf.Max(0, _dropTable[i].Weight);

            if (totalWeight <= 0) return null;

            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int weightSum = 0;

            for (int i = 0; i < _dropTable.Count; i++)
            {
                WeightedPoolItem entry = _dropTable[i];
                int weight = Mathf.Max(0, entry.Weight);
                if (weight == 0) continue;

                weightSum += weight;
                if (randomValue < weightSum)
                    return entry.Item;
            }

            return null;
        }
    }
}
