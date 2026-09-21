using System.Collections.Generic;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Visual;
using Chuh007Lib.Bus;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Work.CHUH.Chuh007Lib.ObjectPool.RunTime;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class Phase2SlashProjectiles
    {
        private readonly Phase2SlashProjectileDashPatternSO _pattern;
        private const float SlashProjectileSoundVolumeMultiplier = 2f;
        public Phase2SlashProjectiles(Phase2SlashProjectileDashPatternSO pattern) => _pattern = pattern;

        public void SpawnSlashProjectile(
            Enemy owner,
            Vector2 direction,
            DamageData damage,
            CancellationToken ct)
        {
            if (_pattern.poolManager == null || _pattern.SlashProjectileItem == null)
                return;

            var pooled = _pattern.poolManager.Pop(_pattern.SlashProjectileItem);
            if (pooled is not IPooledVFX vfx)
            {
                if (pooled != null)
                    _pattern.poolManager.Push(pooled);

                return;
            }

            Vector2 fireDirection = Phase2SlashTargets.NormalizeDirection(direction);
            Vector2 spawnPosition = (Vector2)owner.transform.position + fireDirection * _pattern.ProjectileSpawnOffset;
            vfx.Play(spawnPosition, _pattern.ProjectileScale, fireDirection);
            BossProjectileOutline.ApplyTo(pooled.gameObject);
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.Boss1QuickSwingAttack,
                SoundType.SFX,
                SlashProjectileSoundVolumeMultiplier));
            MoveSlashProjectile(pooled, owner, fireDirection, damage, ct).Forget();
        }

        public async UniTaskVoid MoveSlashProjectile(
            IPoolable pooled,
            Enemy owner,
            Vector2 direction,
            DamageData damage,
            CancellationToken ct)
        {
            if (pooled == null)
                return;

            HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
            Transform projectileTransform = pooled.gameObject.transform;
            float elapsed = 0f;

            while (elapsed < _pattern.ProjectileLifeTime && pooled.gameObject.activeSelf)
            {
                bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
                if (canceled)
                    break;

                float deltaTime = Time.deltaTime;
                Vector2 from = projectileTransform.position;
                Vector2 to = from + direction * (_pattern.ProjectileSpeed * deltaTime);
                _pattern.Targets.TryDamageTargets(owner, damage, from, to, direction, _pattern.ProjectileHitRadius, damagedTargets);
                projectileTransform.position = to;
                elapsed += deltaTime;
            }

            if (pooled.gameObject.activeSelf)
                _pattern.poolManager.Push(pooled);
        }
    }
}
