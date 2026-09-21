using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Audio;
using _Work.CHUH.Code.Combat.Warning;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.EntityPlus;
using Chuh007Lib.Bus;
using Chuh007Lib.Entities.Entities;
using Chuh007Lib.StatSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    [CreateAssetMenu(fileName = "CircleAttackPattern", menuName = "SO/Pattern/CircleAttackPattern", order = 0)]
    public class CircleAttackPattern : BasePatternSO
    {
        [SerializeField] private ContactFilter2D whatIsTarget;
        [SerializeField] private float warningRadius = 1f;
        [SerializeField, Min(0f)] private float damageMultiplier = 1.25f;
        
        private Collider2D[] _colliders = new Collider2D[10];

        protected override PatternAttackType DefaultAttackType => PatternAttackType.Circle;
        
        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            owner.GetCompo<EntityMover>().StopImmediately();

            var warning = poolManager.Pop(warningItem) as CircleWarning;
            warning.Setup(owner.transform.position, warningRadius);
            await warning.PlayAsync(warningDuration, ct);
        }

        protected override async UniTask OnExecutePattern(Enemy owner, CancellationToken ct)
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.Boss1SwingAttack,
                SoundType.SFX));

            DamageData damage = GetAttackDamage(owner, damageMultiplier);
            await UniTask.WaitForSeconds(0.1f, cancellationToken: ct);

            int cnt = Physics2D.OverlapCircle(owner.transform.position, warningRadius, whatIsTarget, _colliders);
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    _colliders[i].GetComponent<IDamageable>()?.TakeDamage(damage);
                }
            }
            await UniTask.WaitForSeconds(0.1f, cancellationToken: ct);
        }

        public UniTask ExecuteAttackOnly(Enemy owner, CancellationToken ct)
        {
            return OnExecutePattern(owner, ct);
        }
    }
}
