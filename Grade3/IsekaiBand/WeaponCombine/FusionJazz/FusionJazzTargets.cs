using System.Collections.Generic;
using _Code.LCH._02.Scripts.Combat;
using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using Chuh007Lib.Entities.Entities;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine
{
    internal sealed class FusionJazzTargets
    {
        private const int MaxTargets = 28;
        private readonly FusionJazzBandAttack _source;
        private readonly FusionJazzStats _stats;
        private readonly Collider2D[] _scanHits = new Collider2D[256];
        private readonly HashSet<int> _scanIds = new();
        private readonly List<TargetCandidate> _targets = new();
        private readonly List<int> _staleTargetIds = new();
        public IReadOnlyList<TargetCandidate> Candidates => _targets;

        public FusionJazzTargets(FusionJazzBandAttack source, FusionJazzStats stats)
        {
            _source = source;
            _stats = stats;
        }

        public void Clear()
        {
            _targets.Clear();
            _staleTargetIds.Clear();
        }

        public void Refresh(Dictionary<int, float> nextFireTimes)
        {
            _targets.Clear();
            _scanIds.Clear();
            int count = Physics2D.OverlapCircle(
                _source.Position,
                _source.ScaleCommonRange(_stats.TargetRange),
                _source.TargetContactFilter,
                _scanHits);

            for (int i = 0; i < count; i++)
            {
                Collider2D hit = _scanHits[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                Entity entity = hit.GetComponent<Entity>() ?? hit.GetComponentInParent<Entity>();
                Transform target = entity != null ? entity.transform : hit.transform.root;
                if (!ManualTargetingService.IsValid(target))
                    continue;

                int id = entity != null ? entity.GetInstanceID() : target.GetInstanceID();
                if (!_scanIds.Add(id))
                    continue;

                _targets.Add(new TargetCandidate(
                    id,
                    target,
                    hit,
                    Vector2.Distance(_source.Position, target.position)));
            }

            _targets.Sort((left, right) => left.Distance.CompareTo(right.Distance));
            if (_targets.Count > MaxTargets)
                _targets.RemoveRange(MaxTargets, _targets.Count - MaxTargets);

            _scanIds.Clear();
            for (int i = 0; i < _targets.Count; i++)
                _scanIds.Add(_targets[i].Id);

            _staleTargetIds.Clear();
            foreach (int targetId in nextFireTimes.Keys)
            {
                if (!_scanIds.Contains(targetId))
                    _staleTargetIds.Add(targetId);
            }
            for (int i = 0; i < _staleTargetIds.Count; i++)
                nextFireTimes.Remove(_staleTargetIds[i]);
        }

        public readonly struct TargetCandidate
        {
            public readonly int Id;
            public readonly Transform Target;
            public readonly Collider2D Collider;
            public readonly float Distance;

            public TargetCandidate(int id, Transform target, Collider2D collider, float distance)
            {
                Id = id;
                Target = target;
                Collider = collider;
                Distance = distance;
            }
        }
    }
}
