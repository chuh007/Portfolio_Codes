using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class PopRockAfterglow
    {
        private const int MaxActiveAfterglowZones = 24;
        private const float AfterglowDuration = 1.8f;
        private const float AfterglowTickInterval = 0.45f;
        private readonly PopRockBandAttack _source;
        private readonly PopRockStats _stats;
        private readonly PopRockAreaDamage _area;
        private readonly BandRuntimeObjects _afterglowZones = new();

        public PopRockAfterglow(PopRockBandAttack source, PopRockStats stats, PopRockAreaDamage area)
        {
            _source = source;
            _stats = stats;
            _area = area;
        }

        public void RemoveDestroyedZones() => _afterglowZones.RemoveDestroyed(includeInactive: false);
        public void Dispose() => _afterglowZones.DestroyAll(reverseOrder: true);

        public void TickAfterglow(Vector3 position)
        {
            _area.Damage(position, _source.ScaleCommonRange(_stats.AfterglowRadius), _stats.AfterglowTickDamage, 0f, true);
            BuildVisualEffect.SpawnCircle(
                position,
                _source.ScaleCommonRange(_stats.AfterglowRadius),
                new Color(1f, 0.16f, 0.58f, 0.28f),
                AfterglowTickInterval + 0.08f,
                2,
                sortingLayerName: GroundEffectRenderLayer.SortingLayerName);
        }

        public void SpawnAfterglowZone(Vector3 position)
        {
            _afterglowZones.RemoveDestroyed(includeInactive: false);
            _afterglowZones.MakeRoom(MaxActiveAfterglowZones);

            var zoneObject = new GameObject("PopRockAfterglowZone");
            zoneObject.transform.position = position;
            zoneObject.AddComponent<PopRockAfterglowZone>().Init(
                _source,
                AfterglowDuration,
                AfterglowTickInterval,
                _source.ScaleCommonRange(_stats.AfterglowRadius));
            _afterglowZones.Add(zoneObject);
        }
    }
}
