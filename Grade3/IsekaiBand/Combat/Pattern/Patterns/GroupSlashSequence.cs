using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class GroupSlashSequence
    {
        private readonly GroupSlashPressurePatternSO _pattern;

        public GroupSlashSequence(GroupSlashPressurePatternSO pattern) => _pattern = pattern;

        public async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            if (owner == null)
            {
                _pattern.LogWarning(null, "Execute aborted: owner is null.");
                return;
            }

            if (_pattern.GroupSlashItem == null)
            {
                _pattern.LogWarning(owner, "Execute aborted: groupSlashItem is null.");
                return;
            }

            DamageData damage = _pattern.GetDamage(owner, _pattern.DamageMultiplier);
            int repeatCount = Mathf.Max(1, _pattern.SlashRepeatCount);

            for (int i = 0; i < repeatCount; i++)
            {
                if (ShouldStop(owner, ct))
                {
                    return;
                }

                Vector2 ownerPosition = owner.transform.position;
                Vector2 targetPosition = _pattern.Aim.GetTargetSlashPosition(owner);
                Vector2 targetDirection = GroupSlashAim.GetDirection(ownerPosition, targetPosition);

                if (_pattern.SlashWarningDuration > 0f)
                {
                    await _pattern.Attack.PlayCircleWarning(targetPosition, _pattern.SlashWarningDuration, ct);
                }

                if (ShouldStop(owner, ct))
                {
                    return;
                }

                _pattern.Attack.SpawnSlash(owner, "TargetSlash", targetPosition, targetDirection, damage, _pattern.TargetSlashScale);

                if (i < repeatCount - 1 && _pattern.SlashInterval > 0f)
                {
                    await UniTask.WaitForSeconds(_pattern.SlashInterval, cancellationToken: ct);
                }
            }
        }

        public static bool ShouldStop(Enemy owner, CancellationToken ct)
        {
            return owner == null || owner.IsDead || ct.IsCancellationRequested;
        }
    }
}
