using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.Visual;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class GroupSlashAttack
    {
        private readonly GroupSlashPressurePatternSO _pattern;
        private const float TargetSlashDurationMultiplier = 2f;
        public GroupSlashAttack(GroupSlashPressurePatternSO pattern) => _pattern = pattern;

        public async UniTask PlayCircleWarning(Vector2 position, float duration, CancellationToken ct)
        {
            if (duration <= 0f)
                return;

            if (_pattern.poolManager == null || _pattern.warningItem == null)
            {
                _pattern.LogWarning(null,
                    $"Warning fallback wait: poolManager={(_pattern.poolManager != null ? _pattern.poolManager.name : "null")}, " +
                    $"warningItem={(_pattern.warningItem != null ? _pattern.warningItem.name : "null")}");
                await UniTask.WaitForSeconds(duration, cancellationToken: ct);
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.warningItem);
            if (pooled is CircleWarning warning)
            {
                warning.Setup(position, _pattern.WarningRadius);
                await warning.PlayAsync(duration, ct);
                return;
            }

            _pattern.LogWarning(null,
                $"Warning pop failed or wrong type. pooled={(pooled != null ? pooled.gameObject.name : "null")}, " +
                $"expected={nameof(CircleWarning)}");
            if (pooled != null)
                _pattern.poolManager.Push(pooled);

            await UniTask.WaitForSeconds(duration, cancellationToken: ct);
        }

        public void SpawnSlash(Enemy owner, string stepName, Vector2 position, Vector2 direction, DamageData damage, float scale)
        {
            if (_pattern.poolManager == null || _pattern.GroupSlashItem == null)
            {
                _pattern.LogWarning(owner,
                    $"{stepName} aborted: poolManager={(_pattern.poolManager != null ? _pattern.poolManager.name : "null")}, " +
                    $"groupSlashItem={(_pattern.GroupSlashItem != null ? _pattern.GroupSlashItem.name : "null")}");
                return;
            }

            var pooled = _pattern.poolManager.Pop(_pattern.GroupSlashItem);
            if (pooled is not IPooledVFX vfx)
            {
                _pattern.LogWarning(owner,
                    $"{stepName} pop failed or wrong type. pooled={(pooled != null ? pooled.gameObject.name : "null")}, " +
                    $"expected={nameof(IPooledVFX)}");
                if (pooled != null)
                    _pattern.poolManager.Push(pooled);

                return;
            }

            Vector2 slashDirection = GroupSlashAim.NormalizeDirection(direction);
            float slashScale = Mathf.Max(0f, scale);
            if (vfx is PooledAnimationVFX animationVfx)
                animationVfx.Play(position, slashScale, slashDirection, TargetSlashDurationMultiplier);
            else
                vfx.Play(position, slashScale, slashDirection);

            VFXDamageBox damageBox = pooled.gameObject.GetComponent<VFXDamageBox>();
            if (damageBox == null)
            {
                _pattern.LogWarning(owner, $"{stepName} spawned without {nameof(VFXDamageBox)}. pooled={pooled.gameObject.name}");
                return;
            }

            float hitRadius = Mathf.Max(0f, _pattern.WarningRadius);
            damageBox.HitCircleRepeatedAndGetCount(
                damage,
                slashDirection,
                owner,
                hitRadius,
                _pattern.DamageTickInterval);
        }
    }
}
