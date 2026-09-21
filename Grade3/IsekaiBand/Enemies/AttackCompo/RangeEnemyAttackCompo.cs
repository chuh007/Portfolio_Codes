using System;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Combat.Projectile;
using Chuh007Lib.ObjectPool.RunTime;
using Chuh007Lib.StatSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Work.CHUH.Code.Enemies.AttackCompo
{
    public class RangeEnemyAttackCompo : EnemyAttackCompo
    {
        private const float RangeEnemyCooldownMultiplier = 2f;
        private const int ShamanProjectileCount = 3;
        private const float ShamanSpreadAngle = 30f;
        private const string ShamanNameKeyword = "Shaman";

        [SerializeField] private StatSO projectileSpeedStat;
        [Header("Pool")]
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO enemyProjectile;
        
        private float _projectileSpeed;

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            _attackCooldown *= RangeEnemyCooldownMultiplier;
            _projectileSpeed = _stat.GetStat(projectileSpeedStat).Value;
        }

        public override void Attack(Action onAttackEnd = null)
        {
            if (TryUseSinglePattern(onAttackEnd)) return;

            base.Attack(onAttackEnd);
            Vector2 dir = _target.transform.position - transform.position;

            if (IsShaman())
            {
                FireSpread(dir);
            }
            else
            {
                FireProjectile(dir);
            }

            onAttackEnd?.Invoke();
        }

        private void FireSpread(Vector2 dir)
        {
            if (dir.sqrMagnitude <= Mathf.Epsilon)
                dir = Vector2.right;

            float startAngle = -ShamanSpreadAngle * 0.5f;
            float angleStep = ShamanSpreadAngle / (ShamanProjectileCount - 1);

            for (int i = 0; i < ShamanProjectileCount; i++)
            {
                FireProjectile(Rotate(dir, startAngle + angleStep * i));
            }
        }

        private void FireProjectile(Vector2 dir)
        {
            EnemyProjectileBase projectile = poolManager.Pop(enemyProjectile) as EnemyProjectileBase;
            Debug.Assert(projectile != null, $"{projectile} is not EnemyProjectile");
            projectile.transform.position = transform.position;
            projectile.InitAndFire(dir, new DamageData(_damage), _projectileSpeed, 5f, _enemy, whatIsTarget);
        }

        private bool IsShaman()
        {
            string enemyName = _enemy.PoolItem != null ? _enemy.PoolItem.poolingName : _enemy.name;
            return !string.IsNullOrEmpty(enemyName) &&
                   enemyName.IndexOf(ShamanNameKeyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static Vector2 Rotate(Vector2 dir, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            return new Vector2(dir.x * cos - dir.y * sin, dir.x * sin + dir.y * cos);
        }
    }
}
