using Code.Core.Pool;
using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    public void ParticlePlay(PollingType type, Vector3 position)
    {
        ParticleItem particleItem = PoolManager.Instance.Pop(type) as ParticleItem;
        
        particleItem.transform.position = position;
        particleItem.PlayEffect();
    }
}
