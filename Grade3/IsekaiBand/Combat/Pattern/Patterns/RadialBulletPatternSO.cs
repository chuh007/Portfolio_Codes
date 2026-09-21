using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Combat.Projectile;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "RadialBulletPattern", menuName = "SO/Pattern/RadialBulletPattern", order = 0)]
    public class RadialBulletPatternSO : BasePatternSO
    {
        [SerializeField] private CircleAttackPattern circleAttackPattern;
        [SerializeField] private PoolItemSO bulletItem;
        [SerializeField] private int ringCount = 3;
        [SerializeField] private int bulletsPerRing = 16;
        [SerializeField] private bool useFireDuration;
        [SerializeField, Min(0f)] private float fireDuration = 6f;
        [SerializeField] private float ringInterval = 0.25f;
        [SerializeField] private float angleOffsetPerRing = 11.25f;
        [SerializeField] private float bulletSpeed = 7f;
        [SerializeField] private float bulletLifeTime = 4f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.6f;
        [SerializeField] private LayerMask whatIsTarget;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Circle;

        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            owner.GetCompo<EntityMover>().StopImmediately();
            if (circleAttackPattern != null)
            {
                await circleAttackPattern.UsePattern(owner, ct, PatternUseMode.PrepareOnly);
                return;
            }

            await base.OnPreparePattern(owner, ct);
        }

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            DamageData damage = GetAttackDamage(owner, damageMultiplier);
            UniTask bulletTask = FireRings(owner, damage, ct);
            if (circleAttackPattern == null)
            {
                await bulletTask;
                return;
            }

            await UniTask.WhenAll(
                circleAttackPattern.ExecuteAttackOnly(owner, ct),
                bulletTask);
        }

        private async UniTask FireRings(Enemy owner, DamageData damage, CancellationToken ct)
        {
            if (useFireDuration)
            {
                var soundPlaybackKey = new object();
                Bus<SoundLoopStartEvent>.Raise(new SoundLoopStartEvent(
                    soundPlaybackKey,
                    SoundKeys.BossSpellCasting));

                try
                {
                    await FireRingsForDuration(owner, damage, ct);
                }
                finally
                {
                    Bus<SoundLoopStopEvent>.Raise(new SoundLoopStopEvent(soundPlaybackKey));
                }
                return;
            }

            int safeRingCount = Mathf.Max(1, ringCount);

            for (int i = 0; i < safeRingCount; i++)
            {
                if (ct.IsCancellationRequested) return;

                FireRing(owner, damage, i * angleOffsetPerRing);

                if (i < safeRingCount - 1 && ringInterval > 0f)
                    await UniTask.WaitForSeconds(ringInterval, cancellationToken: ct);
            }
        }

        private async UniTask FireRingsForDuration(Enemy owner, DamageData damage, CancellationToken ct)
        {
            if (fireDuration <= 0f) return;

            float elapsed = 0f;
            int ringIndex = 0;

            while (elapsed < fireDuration)
            {
                if (ct.IsCancellationRequested) return;

                FireRing(owner, damage, ringIndex * angleOffsetPerRing);
                ringIndex++;

                if (ringInterval <= 0f) return;

                float waitTime = Mathf.Min(ringInterval, fireDuration - elapsed);
                if (waitTime <= 0f) return;

                await UniTask.WaitForSeconds(waitTime, cancellationToken: ct);
                elapsed += waitTime;
            }
        }

        private void FireRing(Enemy owner, DamageData damage, float startAngle)
        {
            int safeBulletCount = Mathf.Max(1, bulletsPerRing);
            float angleStep = 360f / safeBulletCount;

            for (int i = 0; i < safeBulletCount; i++)
            {
                float angle = startAngle + angleStep * i;
                float rad = angle * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                SpawnBullet(owner, dir, damage);
            }
        }

        private void SpawnBullet(Enemy owner, Vector2 dir, DamageData damage)
        {
            EnemyProjectileBase projectile = poolManager.Pop(bulletItem) as EnemyProjectileBase;
            Debug.Assert(projectile != null, $"{bulletItem.name} is not EnemyProjectileBase");
            projectile.transform.position = owner.transform.position;
            projectile.InitAndFire(dir, damage, bulletSpeed, bulletLifeTime, owner, whatIsTarget);
        }

    }
}
