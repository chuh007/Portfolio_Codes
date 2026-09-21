using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Combat.Projectile;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "FireBulletPattern", menuName = "SO/Pattern/FireBulletPattern", order = 0)]
    public class FireBulletPattern : BasePatternSO
    {
        [SerializeField] private PoolItemSO bulletItem;
        [SerializeField] private float totalDuration = 5f;
        [SerializeField] private float fireInterval = 0.2f;
        [SerializeField] private int bulletCount = 3;
        [SerializeField] private float angleSpread = 30f;
        [SerializeField] private float bulletSpeed = 5f;
        [SerializeField] private float bulletLifeTime = 3f;
        [SerializeField, Min(0f)] private float damageMultiplier = 0.6f;
        [SerializeField] private LayerMask whatIsTarget;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.FireBullet;

        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            owner.GetCompo<EntityMover>().StopImmediately();
            await base.OnPreparePattern(owner, ct);
        }

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            DamageData damage = GetAttackDamage(owner, damageMultiplier);

            if (ct.IsCancellationRequested) return;

            float elapsed = 0f;
            while (elapsed < totalDuration)
            {
                if (ct.IsCancellationRequested) return;
                FireBullets(owner, damage);
                await UniTask.WaitForSeconds(fireInterval, cancellationToken: ct);
                elapsed += fireInterval;
            }
        }

        private void FireBullets(Enemy owner, DamageData damage)
        {
            Vector2 baseDir = (owner.target.transform.position - owner.transform.position).normalized;
            float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

            if (bulletCount == 1)
            {
                SpawnBullet(owner, baseDir, damage);
                return;
            }

            for (int i = 0; i < bulletCount; i++)
            {
                float t = (float)i / (bulletCount - 1); // 0 ~ 1
                float angle = baseAngle + Mathf.Lerp(-angleSpread, angleSpread, t);
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
