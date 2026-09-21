using System.Collections.Generic;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.StatSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal class LaneSweepPlan
    {
        private readonly Phase2LaneSweepPatternSO _pattern;
        private const int LaneCount = 3;
        private const int AllDangerMask = (1 << LaneCount) - 1;
        public Rect ArenaBounds;
        public List<LaneSweep> Sweeps { get; } = new();
        public LaneSweepPlan(Phase2LaneSweepPatternSO pattern) => _pattern = pattern;

        public void BuildDangerMasks()
        {
            _pattern.Plan.Sweeps.Clear();
            for (int i = 0; i < _pattern.SweepCount; i++)
            {
                int previousMask = _pattern.Plan.Sweeps.Count > 0 ? _pattern.Plan.Sweeps[^1].Mask : -1;
                _pattern.Plan.Sweeps.Add(BuildSweep(PickDangerMask(previousMask)));
            }
        }

        public LaneSweep BuildSweep(int dangerMask)
        {
            var laneRects = new List<Rect>(LaneCount);
            for (int lane = 0; lane < LaneCount; lane++)
            {
                if (HasLane(dangerMask, lane))
                    laneRects.Add(GetLaneRect(lane));
            }

            return new LaneSweep(dangerMask, laneRects.ToArray());
        }

        public static int PickDangerMask(int previousMask)
        {
            int mask = Random.Range(1, AllDangerMask);
            return mask != previousMask ? mask : mask == 1 ? 2 : mask - 1;
        }

        public Rect GetLaneRect(int lane)
        {
            float height = _pattern.Plan.ArenaBounds.height / LaneCount;
            return new Rect(_pattern.Plan.ArenaBounds.xMin, _pattern.Plan.ArenaBounds.yMin + height * lane, _pattern.Plan.ArenaBounds.width, height);
        }

        public bool TryGetArenaBounds(out Rect bounds)
        {
            StageHelper stageHelper = StageHelper.Instance;
            if (stageHelper == null || !stageHelper.IsBossArenaActive)
            {
                bounds = default;
                return false;
            }

            Vector2 size = Vector2.Max(Vector2.zero, stageHelper.BossArenaSize - Vector2.one * (_pattern.MapPadding * 2f));
            bounds = new Rect(stageHelper.BossArenaCenter - size * 0.5f, size);
            return bounds.width > Mathf.Epsilon && bounds.height > Mathf.Epsilon;
        }

        public static Rect GetSlashSegmentRect(Rect laneRect, int slashIndex, int slashCount)
        {
            float width = laneRect.width / slashCount;
            float xMax = laneRect.xMax - width * slashIndex;
            return new Rect(xMax - width, laneRect.yMin, width, laneRect.height);
        }

        public static bool HasLane(int mask, int lane)
        {
            return (mask & (1 << lane)) != 0;
        }
    }
}
