using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    public sealed class PunkBandAttack : CombineWeaponBase
    {
        private const float SpikeHalfAngle = 7.5f;

        private readonly PunkStats _stats = new();
        private readonly Collider2D[] _hits = new Collider2D[192];
        private readonly HashSet<int> _hitIds = new();

        private float _spikeTimer;
        private PunkSpectrumVisual _spectrumVisual;

        public override CombineWeaponType CombinationType => CombineWeaponType.PunkBand;
        public override WeaponType PrimaryWeaponType => WeaponType.Bass;

        public override void ConfigureIngredients(IReadOnlyDictionary<WeaponType, PlayerAttackBase> ingredients)
            => _stats.Configure(ingredients);

        public override void Tick(float deltaTime)
        {
            EnsureSpectrumVisual();
            _spikeTimer += deltaTime;

            float interval = ScaleCommonInterval(_stats.SpikeInterval);
            while (_spikeTimer >= interval)
            {
                _spikeTimer -= interval;
                FireSpectrumSpikes();
            }
        }

        public override void Dispose()
        {
            if (_spectrumVisual != null)
                Object.Destroy(_spectrumVisual.gameObject);
            _spectrumVisual = null;
            base.Dispose();
        }

        protected override void OnAttack()
        {
        }

        private void FireSpectrumSpikes()
        {
            float[] spikeAngles = PunkSpikePattern.CreateAngles(
                OwnerPosition, ScaleCommonRange(_stats.BaseRadius) * 0.2f, ScaleCommonRange(_stats.SpikeLength), TargetContactFilter, _hits);
            foreach (float angle in spikeAngles)
                DamageSpike(angle);

            EnsureSpectrumVisual();
            _spectrumVisual?.SetDamageSpikes(
                spikeAngles,
                ScaleCommonRange(_stats.SpikeLength),
                _stats.SpikeInterval * 0.45f);
        }

        private void DamageSpike(float angle)
        {
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            _hitIds.Clear();
            int count = Physics2D.OverlapCircle(OwnerPosition, ScaleCommonRange(_stats.SpikeLength), TargetContactFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _hits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                int targetId = BandRuntimeVisuals.GetTargetId(hit);
                if (!_hitIds.Add(targetId))
                    continue;

                Vector2 toTarget = (Vector2)hit.transform.position - (Vector2)OwnerPosition;
                float distance = toTarget.magnitude;
                if (distance < ScaleCommonRange(_stats.BaseRadius) * 0.2f
                    || distance > ScaleCommonRange(_stats.SpikeLength)
                    || Vector2.Angle(direction, toTarget) > SpikeHalfAngle)
                    continue;

                DamageEnemy(hit, OwnerPosition, _stats.Damage, ScaleCommonRange(_stats.SpikeLength), _stats.Knockback);
            }
        }

        private void EnsureSpectrumVisual()
        {
            if (_spectrumVisual == null)
            {
                var obj = new GameObject("PunkBandAudioSpectrum");
                _spectrumVisual = obj.AddComponent<PunkSpectrumVisual>();
            }

            _spectrumVisual.InitOrUpdate(
                OwnerTransform,
                ScaleCommonRange(_stats.BaseRadius),
                new Color(1f, 0.12f, 0.52f, 0.86f));
        }
    }

}
