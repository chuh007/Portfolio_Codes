using System.Collections.Generic;
using _Work.CHUH.Code.EntityPlus;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class SlowFieldTargets
    {
        private readonly SlowFieldZone _zone;
        private readonly Collider2D[] _hits = new Collider2D[16];
        private readonly HashSet<EntityStat> _activeStats = new HashSet<EntityStat>();
        private readonly HashSet<EntityStat> _currentStats = new HashSet<EntityStat>();
        private readonly List<EntityStat> _removeBuffer = new List<EntityStat>();


        public SlowFieldTargets(SlowFieldZone zone) => _zone = zone;

        public void RefreshSlowTargets()
        {
            _currentStats.Clear();

            int count = Physics2D.OverlapCircle(_zone.transform.position, _zone.Radius, _zone.TargetFilter, _hits);
            for (int i = 0; i < count; i++)
            {
                EntityStat stat = FindEntityStat(_hits[i]);
                if (stat == null || !stat.TryGetStatByName(_zone.MoveSpeedStatName, out _))
                    continue;

                _currentStats.Add(stat);
                if (_activeStats.Add(stat))
                    stat.AddPercentModifierByName(_zone.MoveSpeedStatName, _zone, _zone.SpeedMultiplier);
            }

            _removeBuffer.Clear();
            foreach (EntityStat stat in _activeStats)
            {
                if (!_currentStats.Contains(stat))
                    _removeBuffer.Add(stat);
            }

            for (int i = 0; i < _removeBuffer.Count; i++)
                RemoveSlow(_removeBuffer[i]);
        }

        public void ClearSlowTargets()
        {
            foreach (EntityStat stat in _activeStats)
                stat?.RemoveModifierByKey(_zone);

            _activeStats.Clear();
            _currentStats.Clear();
            _removeBuffer.Clear();
        }

        public void RemoveSlow(EntityStat stat)
        {
            if (stat == null)
                return;

            stat.RemoveModifierByKey(_zone);
            _activeStats.Remove(stat);
        }

        public static EntityStat FindEntityStat(Collider2D hit)
        {
            if (hit == null)
                return null;

            if (hit.TryGetComponent(out EntityStat stat))
                return stat;

            stat = hit.GetComponentInParent<EntityStat>();
            if (stat != null)
                return stat;

            if (hit.attachedRigidbody != null && hit.attachedRigidbody.TryGetComponent(out stat))
                return stat;

            return hit.GetComponentInChildren<EntityStat>();
        }
    }
}
