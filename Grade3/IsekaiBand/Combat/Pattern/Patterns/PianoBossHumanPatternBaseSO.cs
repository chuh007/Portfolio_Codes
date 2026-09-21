using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    public abstract class PianoBossHumanPatternBaseSO : BasePatternSO
    {
        [Header("Target")]
        [SerializeField] protected LayerMask whatIsTarget;
        [SerializeField] private DamageType damageType = DamageType.Physical;

        [Header("Note Visual")]
        [SerializeField] private Sprite[] noteSprites;
        [SerializeField] protected int visualSortingOrder = 10;

        protected override PatternAttackType DefaultAttackType => PatternAttackType.FireBullet;

        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            StopOwner(owner);
            await base.OnPreparePattern(owner, ct);
        }

        protected DamageData CreateNoteDamage(Enemy owner, float multiplier)
        {
            DamageData damage = GetAttackDamage(owner, multiplier);
            damage.DamageType = damageType;
            return damage;
        }

        protected Sprite GetNoteSprite(int index)
        {
            if (noteSprites == null || noteSprites.Length == 0)
                return null;

            return noteSprites[Mathf.Abs(index) % noteSprites.Length];
        }

        protected static void StopOwner(Enemy owner)
        {
            owner?.GetCompo<EntityMover>()?.StopImmediately();
        }
    }
}
