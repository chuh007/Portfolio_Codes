using _Code.LCH._02.Scripts.Card.Build;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.EntityPlus.Effect.EffectData;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.EntityPlus.Effect
{
    public class DebuffEffect : EntityEffect
    {
        public DebuffType Type => ((DebuffEffectSO)EffectData).type;
        public PlayerCommonBuildCompo SourceBuilds { get; private set; }
        public float Value { get; private set; }
        public float TickDamage { get; private set; }
        public float AppliedDuration { get; private set; }
        public bool CanSpread { get; private set; }
        public override int MaxStack => _maxStack;
        protected override float TickDelay => _tickInterval;

        private int _maxStack;
        private float _tickInterval;
        private float _slowMultiplier = 1f;
        private EntityMover _mover;
        private StunEffectRuntime _stun;

        public DebuffEffect(DebuffEffectSO data, Entity target, Entity source = null)
            : base(data, target, source)
        {
            SourceBuilds = source != null ? source.GetComponent<PlayerCommonBuildCompo>() : null;
            Value = data.value;
            TickDamage = data.tickDamage;
            _tickInterval = data.tickInterval;
            _maxStack = Mathf.Max(1, data.maxStack);
            AppliedDuration = CommonBuildDebuffApplier.GetDuration(data.duration, data.isHardCC, SourceBuilds);
            Duration = AppliedDuration;
            CanSpread = data.canSpread || (SourceBuilds != null && SourceBuilds.GetLevel(CommonBuildType.DebuffSpread) > 0);
        }

        public override bool Matches(AbstractEffectDataSO data)
        {
            return data is DebuffEffectSO debuff && debuff.type == Type;
        }

        public override void Activate()
        {
            if (Type == DebuffType.Stun)
            {
                _stun = EffectTarget.GetComponent<StunEffectRuntime>()
                    ?? EffectTarget.gameObject.AddComponent<StunEffectRuntime>();
                _stun.BeginStun();
            }
            else if (Type == DebuffType.Slow)
                _mover = EffectTarget.GetComponentInChildren<EntityMover>();

            RefreshStatus();
        }

        public override void RefreshFrom(EntityEffect effect)
        {
            var debuff = (DebuffEffect)effect;
            EffectData = debuff.EffectData;
            Source = debuff.Source;
            SourceBuilds = debuff.SourceBuilds;
            Value = debuff.Value;
            TickDamage = debuff.TickDamage;
            _tickInterval = debuff._tickInterval;
            _maxStack = debuff.MaxStack;
            AppliedDuration = debuff.AppliedDuration;
            Duration = Mathf.Max(Duration, debuff.Duration);
            CanSpread = debuff.CanSpread;
            AddStack(EffectData.stackPolicy == StackPolicy.Stack ? 1 : 0);
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            if (_mover != null)
            {
                _slowMultiplier = Mathf.Min(_slowMultiplier, 1f - Mathf.Clamp01(Value * CurrentStack));
                _mover.SetMoveSpeedModifier(this, _slowMultiplier);
            }

            CommonBuildDebuffApplier.TryApplyDebuffOverdrive(
                EffectTarget.GetCompo<EntityEffectController>(), SourceBuilds);
        }

        public override void Deactivate()
        {
            _mover?.RemoveMoveSpeedModifier(this);
            _stun?.EndStun();
            base.Deactivate();
        }

        protected override void OnTick()
        {
            if (TickDamage > 0f)
                EffectTarget.TakeDamage(new DamageData(TickDamage * CurrentStack), dealer: Source);
        }

        public override void OnTargetDeath()
        {
            DebuffEffectSpread.Spread(this);
        }

        public DebuffEffect CreateSpreadEffect(Entity target)
        {
            float durationMultiplier = DebuffEffects.IsBoss(EffectTarget) ? 0.35f
                : DebuffEffects.IsElite(EffectTarget) ? 0.45f : 0.5f;
            var effect = new DebuffEffect((DebuffEffectSO)EffectData, target, Source);
            effect.AppliedDuration = Mathf.Max(0.2f, AppliedDuration * durationMultiplier);
            effect.Duration = effect.AppliedDuration;
            effect.Value = Value * 0.65f;
            effect.TickDamage = TickDamage * 0.55f;
            effect._maxStack = 1;
            // 이미 보정된 효과를 전파한다. 숙련 보너스와 전파 권한을 다시 붙이지 않는다.
            effect.CanSpread = false;
            return effect;
        }
    }
}
