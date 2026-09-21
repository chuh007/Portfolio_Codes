using _Code.LCH._02.Scripts.Player.Attack;
using _Work.CHUH.Code.Audio;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class OrthodoxRockProjectiles
    {
        private const int RockCount = 28;
        private readonly OrthodoxRockBandAttack _source;
        private readonly OrthodoxRockStats _stats;
        private readonly BandRuntimeObjects _rocks;
        private readonly BandRuntimeObjects _shockwaves;

        public OrthodoxRockProjectiles(OrthodoxRockBandAttack source, OrthodoxRockStats stats,
            BandRuntimeObjects rocks, BandRuntimeObjects shockwaves)
        {
            _source = source;
            _stats = stats;
            _rocks = rocks;
            _shockwaves = shockwaves;
        }

        public void FireRockVolley()
        {
            int rockCount = _source.CountProjectiles(RockCount);
            float startAngle = Random.Range(0f, 360f / rockCount);
            float travelRange = Mathf.Max(_source.ScaleCommonRange(_stats.RockRange), _source.ScaleCommonRange(_stats.VenueRadius) * 1.15f);
            for (int i = 0; i < rockCount; i++)
            {
                float angle = startAngle + 360f * i / rockCount;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                GameObject obj = BandRuntimeVisuals.SpawnProjectile(
                    null,
                    "OrthodoxRockProjectile",
                    _source.Position,
                    Quaternion.Euler(0f, 0f, angle),
                    new Color(0.82f, 0.65f, 0.42f, 1f),
                    1.75f,
                    55);
                if (obj == null)
                    continue;

                obj.name = "OrthodoxRockProjectile";
                obj.transform.localScale = Vector3.one * 1.75f;
                foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>(true))
                    renderer.color = new Color(1f, 0.42f, 0.16f, 1f);
                ProjectileRenderLayer.ApplyTo(obj);
                (obj.GetComponent<OrthodoxRockProjectile>() ?? obj.AddComponent<OrthodoxRockProjectile>()).Init(
                    _source,
                    direction,
                    _stats.RockSpeed,
                    travelRange,
                    _stats.RockDamage,
                    0.42f);
                _rocks.Add(obj);
            }
        }

        public void FireShockwave()
        {
            Bus<SoundPlayEvent>.Raise(new SoundPlayEvent(
                SoundKeys.DrumShockwave,
                SoundType.SFX,
                suppressDuplicateThisFrame: true));

            if (!DrumWaveAnimationEffect.Spawn(
                    _source.Position, _source.ScaleCommonRange(_stats.VenueRadius),
                    new Color(1f, 0.72f, 0.15f, 0.72f), 0.75f, 55))
            {
                BuildVisualEffect.SpawnCircle(
                    _source.Position, _source.ScaleCommonRange(_stats.VenueRadius),
                    new Color(1f, 0.72f, 0.15f, 0.55f), 0.75f, 55);
            }

            var obj = new GameObject("OrthodoxRockShockwave");
            obj.transform.position = _source.Position;
            var shockwave = obj.AddComponent<OrthodoxRockShockwave>();
            shockwave.Init(_source, _source.ScaleCommonRange(_stats.VenueRadius), _source.ScaleCommonRange(_stats.VenueRadius) / 0.75f, 0.22f);
            _shockwaves.Add(obj);
        }
    }
}
