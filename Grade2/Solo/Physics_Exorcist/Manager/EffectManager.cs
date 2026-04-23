using _01Scripts.Core.EventSystem;
using Blade.Effects;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;

namespace _01Scripts.Manager
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO spawnChannel;
        [SerializeField] private PoolManagerSO poolManager;

        private void Awake()
        {
            spawnChannel.AddListener<SpawnAnimationEffect>(HandleSpawnAnimationEffect);
        }

        private void OnDestroy()
        {
            spawnChannel.RemoveListener<SpawnAnimationEffect>(HandleSpawnAnimationEffect);             
        }

        private void HandleSpawnAnimationEffect(SpawnAnimationEffect evt)
        { 
            PoolingEffect effect = poolManager.Pop(evt.poolType) as PoolingEffect;
            effect.PlayVFX(evt.position, evt.rotation);
        }
    }
}
