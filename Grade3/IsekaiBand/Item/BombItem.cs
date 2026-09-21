using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Visual;
using Chuh007Lib.Bus;
using Chuh007Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Item
{
    public class BombItem : ItemBase
    {
        private enum BurstMode
        {
            Particle,
            Animation,
            Both
        }

        [SerializeField] private float damage = 100f;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private BurstMode burstMode = BurstMode.Particle;
        [SerializeField] private PoolItemSO particleBurst;
        [SerializeField] private PoolItemSO animationBurst;
        [SerializeField] private float burstScale = 6f;

        protected override void OnCollect()
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.BombItemExplosion,
                SoundType.SFX));
            PlayBurst();
            DamageAllEnemies();
        }

        private void PlayBurst()
        {
            if (poolManager == null) return;

            if (burstMode is BurstMode.Particle or BurstMode.Both)
                PlayBurst(particleBurst);

            if (burstMode is BurstMode.Animation or BurstMode.Both)
                PlayBurst(animationBurst);
        }

        private void PlayBurst(PoolItemSO burstPoolItem)
        {
            if (burstPoolItem == null) return;
            
            var pooled = poolManager.Pop(burstPoolItem);
            if (pooled is not IPooledVFX vfx)
            {
                Debug.LogWarning($"[BombItem] {burstPoolItem.name} prefab needs IPooledVFX.");
                if (pooled != null)
                    poolManager.Push(pooled);
                return;
            }

            vfx.Play(transform.position, burstScale, Vector2.right);
        }

        private void DamageAllEnemies()
        {
            var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            var damageData = new DamageData(damage);
            
            foreach (var enemy in enemies)
            {
                if (enemy == null || enemy.IsDead) continue;
                
                Vector2 direction = enemy.transform.position - transform.position;
                enemy.TakeDamage(damageData, direction.normalized);
            }
        }
    }
}
