using System.Collections.Generic;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class RhythmSectionAttack : CombineWeaponBase
    {
        private const float HeavyShockwaveInterval = 7f;
        private const float HeavyShockwaveRadius = 4.2f;
        private const float HeavyShockwaveKnockback = 1f;

        private readonly RhythmSectionStats _stats = new();
        private readonly Collider2D[] _hits = new Collider2D[96];
        private float _heavyShockwaveTimer;
        private BassPrototypeAttack.BassLowZoneLoopVisualRuntime _zoneVisual;

        public override CombineWeaponType CombinationType => CombineWeaponType.RhythmSection;
        public override WeaponType PrimaryWeaponType => WeaponType.Bass;

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => SetRuntimeStat(AttackStatName.Cooldown, this, _stats.Configure(ingredients));

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            UpdateHeavyShockwave(deltaTime);
            UpdateZoneVisual();
        }

        public override void Dispose()
        {
            ClearZoneVisual();
            base.Dispose();
        }

        protected override void OnAttack()
        {
            DamageEnemiesInRadius(ScaleCommonRange(_stats.Radius), _stats.Damage, _stats.Knockback);

            BandRuntimeVisuals.SpawnSignatureBurst(
                OwnerPosition,
                ScaleCommonRange(_stats.Radius),
                new Color(0.12f, 1f, 0.5f, 0.52f),
                new Color(1f, 0.78f, 0.18f, 0.68f),
                4,
                45f,
                0.3f,
                51);
        }

        private void UpdateHeavyShockwave(float deltaTime)
        {
            _heavyShockwaveTimer += deltaTime;
            float interval = ScaleCommonInterval(HeavyShockwaveInterval);
            while (_heavyShockwaveTimer >= interval)
            {
                _heavyShockwaveTimer -= interval;
                EmitHeavyShockwave();
            }
        }

        private void EmitHeavyShockwave()
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.DrumShockwave,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));

            DamageEnemiesInRadius(ScaleCommonRange(HeavyShockwaveRadius), _stats.Damage, HeavyShockwaveKnockback);

            BandRuntimeVisuals.SpawnSignatureBurst(
                OwnerPosition,
                ScaleCommonRange(HeavyShockwaveRadius),
                new Color(1f, 0.76f, 0.12f, 0.62f),
                new Color(0.18f, 1f, 0.5f, 0.72f),
                10,
                18f,
                0.46f,
                55);
        }

        private void DamageEnemiesInRadius(float radius, float damage, float knockback)
        {
            int count = Physics2D.OverlapCircle(OwnerPosition, radius, TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy")) continue;
                DamageEnemy(hit, OwnerPosition, damage, radius, knockback);
            }

            RaiseImpact(new AttackEventContext(
                this, null, OwnerPosition, Vector2.zero, damage, radius, radius,
                0f, 0f, 0, 0f, knockback));
        }

        private void UpdateZoneVisual()
        {
            if (Owner == null)
            {
                ClearZoneVisual();
                return;
            }

            if (_zoneVisual == null)
            {
                var obj = new GameObject("RhythmSectionZoneLoopFx");
                _zoneVisual = obj.AddComponent<BassPrototypeAttack.BassLowZoneLoopVisualRuntime>();
            }

            _zoneVisual.InitOrUpdate(
                Owner,
                ScaleCommonRange(_stats.Radius),
                new Color(0.12f, 0.82f, 0.62f, 0.4f),
                46);
        }

        private void ClearZoneVisual()
        {
            if (_zoneVisual == null) return;

            Object.Destroy(_zoneVisual.gameObject);
            _zoneVisual = null;
        }
    }
}
