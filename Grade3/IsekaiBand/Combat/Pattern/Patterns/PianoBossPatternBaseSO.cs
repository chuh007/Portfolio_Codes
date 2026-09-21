using System;
using System.Threading;
using _Code.LCH._02.Scripts.Combat;
using _Work.CHUH.Code.Enemies;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Entities.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    public abstract class PianoBossPatternBaseSO : BasePatternSO
    {
        [Header("Target")]
        [SerializeField] protected LayerMask whatIsTarget;
        [SerializeField] private DamageType damageType = DamageType.Physical;

        [Header("Visual")]
        [SerializeField] protected Sprite keySprite;
        [SerializeField] protected Sprite[] noteSprites = Array.Empty<Sprite>();
        [SerializeField] protected int visualSortingOrder = 2;

        [Header("Arena")]
        [SerializeField, Min(0f)] private float mapPadding = 1f;
        [SerializeField] private Vector2 fallbackArenaSize = new Vector2(18f, 10f);

        protected override PatternAttackType DefaultAttackType => PatternAttackType.FireBullet;
        protected virtual float FloorDamageMultiplier => 0.58f;

        protected override async UniTask OnPreparePattern(Enemy owner, CancellationToken ct)
        {
            StopOwner(owner);

            PianoBossRuntime runtime = GetRuntime(owner);
            runtime.PulsePerformance(warningDuration);
            EnsurePianoFloor(owner, runtime, CreateDamage(owner, FloorDamageMultiplier));

            await base.OnPreparePattern(owner, ct);
        }

        protected PianoBossRuntime GetRuntime(Enemy owner)
        {
            PianoBossRuntime runtime = owner.GetComponent<PianoBossRuntime>();
            if (runtime == null)
                runtime = owner.gameObject.AddComponent<PianoBossRuntime>();

            runtime.EnsureBound(owner);
            return runtime;
        }

        protected void EnsurePianoFloor(Enemy owner, PianoBossRuntime runtime, DamageData floorDamage)
        {
            runtime.EnsurePhase2Floor(GetArenaBounds(owner), keySprite, floorDamage, whatIsTarget);
        }

        protected Rect GetArenaBounds(Enemy owner)
        {
            StageHelper helper = StageHelper.Instance;
            if (helper != null && helper.IsBossArenaActive)
            {
                Vector2 size = Vector2.Max(Vector2.one, helper.BossArenaSize - Vector2.one * (mapPadding * 2f));
                return new Rect(helper.BossArenaCenter - size * 0.5f, size);
            }

            Vector2 center = owner.target != null ? owner.target.transform.position : owner.transform.position;
            Vector2 fallbackSize = Vector2.Max(Vector2.one, fallbackArenaSize);
            return new Rect(center - fallbackSize * 0.5f, fallbackSize);
        }

        protected DamageData CreateDamage(Enemy owner, float multiplier)
        {
            DamageData damage = GetAttackDamage(owner, multiplier);
            damage.DamageType = damageType;
            return damage;
        }

        protected Sprite GetNoteSprite(int index)
        {
            if (noteSprites != null && noteSprites.Length > 0)
            {
                Sprite sprite = noteSprites[Mathf.Abs(index) % noteSprites.Length];
                if (sprite != null)
                    return sprite;
            }

            return keySprite;
        }

        protected static Sprite ResolveSprite(Sprite primary, Sprite fallback)
        {
            return primary != null ? primary : fallback;
        }

        private static void StopOwner(Enemy owner)
        {
            EntityMover mover = owner.GetCompo<EntityMover>();
            if (mover == null)
                return;

            mover.StopImmediately();
            mover.CanManualMove = false;
        }
    }
}
