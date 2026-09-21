using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class JazzBandBassZones
    {
        private const int MaxActiveBassZones = 24;
        private const float BassZoneDuration = 1f;
        private const float BassZoneTickInterval = 0.2f;
        private readonly JazzBandAttack _source;
        private readonly JazzBandStats _stats;
        private readonly JazzBandAreaAttack _areaAttack;
        private readonly BandRuntimeObjects _bassZones = new();

        public JazzBandBassZones(JazzBandAttack source, JazzBandStats stats, JazzBandAreaAttack areaAttack)
        {
            _source = source;
            _stats = stats;
            _areaAttack = areaAttack;
        }

        public void RemoveDestroyedZones() => _bassZones.RemoveDestroyed();
        public void Dispose() => _bassZones.DestroyAll(reverseOrder: true);

        public void TickBassZone(Vector3 position)
        {
            _areaAttack.DamageArea(
                position,
                _source.ScaleCommonRange(_stats.BassZoneRadius),
                _stats.BassZoneTickDamage,
                0f,
                true);
            BuildVisualEffect.SpawnCircle(
                position,
                _source.ScaleCommonRange(_stats.BassZoneRadius),
                new Color(0.14f, 0.82f, 0.68f, 0.34f),
                BassZoneTickInterval + 0.05f,
                3,
                sortingLayerName: GroundEffectRenderLayer.SortingLayerName);
        }

        public void SpawnBassZone(Vector3 position)
        {
            _bassZones.RemoveDestroyed();
            _bassZones.MakeRoom(MaxActiveBassZones);

            var obj = new GameObject("JazzBandBassZone");
            obj.transform.position = position;
            var zone = obj.AddComponent<JazzBandBassZoneRuntime>();
            zone.Init(_source, BassZoneDuration, BassZoneTickInterval);
            _bassZones.Add(obj);
        }
    }
}
