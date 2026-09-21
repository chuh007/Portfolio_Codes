using UnityEngine;

namespace _Work.CHUH.Code.Combat.Pattern.Patterns
{
    internal readonly struct LaneSweep
    {
            public readonly int Mask;
            public readonly Rect[] LaneRects;

            public LaneSweep(int mask, Rect[] laneRects)
            {
                Mask = mask;
                LaneRects = laneRects;
            }

    }
}
