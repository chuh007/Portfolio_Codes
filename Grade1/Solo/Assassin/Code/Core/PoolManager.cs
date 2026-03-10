using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Pool
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [SerializeField] private ParticleItem _particle;

        private Dictionary<PollingType, Pool> _pools;

        private void Awake()
        {
            _pools = new Dictionary<PollingType, Pool>();
            Pool DeadParticlePool = new Pool(_particle, transform, 5);
            _pools.Add(PollingType.EnemyDeadParticle, DeadParticlePool);
        }

        public IPoolable Pop(PollingType poolType)
        {
            IPoolable item = _pools[poolType].Pop();
            item.ResetItem();
            return item;
        }

        public void Push(IPoolable item)
        {
            _pools[item.PoolType].Push(item);
        }
    }
}

