using UnityEngine;

namespace Code.Core.Pool
{
    public interface IPoolable
    {
        public PollingType PoolType { get; }
        public GameObject ObjectPrefab { get; }
        public void ResetItem();
    }

    public enum PollingType
    {
        EnemyDeadParticle
    }
}



