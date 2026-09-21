using System.Collections.Generic;

namespace _Work.CHUH.Code.StageSystem
{
    internal static class DecorationRuleSelector
    {
        public static bool HasGround(IReadOnlyList<GroundDecorationRule> groundDecorations)
        {
            if (groundDecorations == null)
                return false;

            for (int i = 0; i < groundDecorations.Count; i++)
            {
                GroundDecorationRule rule = groundDecorations[i];
                if (rule != null && rule.IsUsable)
                    return true;
            }

            return false;
        }

        public static bool HasLarge(IReadOnlyList<LargeDecorationRule> largeDecorations)
        {
            if (largeDecorations == null)
                return false;

            for (int i = 0; i < largeDecorations.Count; i++)
            {
                LargeDecorationRule rule = largeDecorations[i];
                if (rule != null && rule.IsUsable)
                    return true;
            }

            return false;
        }

        public static GroundDecorationRule SelectGround(IReadOnlyList<GroundDecorationRule> groundDecorations, uint roll)
        {
            if (groundDecorations == null)
                return null;

            int totalWeight = 0;
            for (int i = 0; i < groundDecorations.Count; i++)
            {
                GroundDecorationRule rule = groundDecorations[i];
                if (rule != null && rule.IsUsable)
                    totalWeight += rule.Weight;
            }

            if (totalWeight <= 0)
                return null;

            int target = (int)(roll % (uint)totalWeight);
            int accumulatedWeight = 0;

            for (int i = 0; i < groundDecorations.Count; i++)
            {
                GroundDecorationRule rule = groundDecorations[i];
                if (rule == null || !rule.IsUsable)
                    continue;

                accumulatedWeight += rule.Weight;
                if (target < accumulatedWeight)
                    return rule;
            }

            return null;
        }

        public static LargeDecorationRule SelectLarge(IReadOnlyList<LargeDecorationRule> largeDecorations, uint roll)
        {
            if (largeDecorations == null)
                return null;

            int totalWeight = 0;
            for (int i = 0; i < largeDecorations.Count; i++)
            {
                LargeDecorationRule rule = largeDecorations[i];
                if (rule != null && rule.IsUsable)
                    totalWeight += rule.Weight;
            }

            if (totalWeight <= 0)
                return null;

            int target = (int)(roll % (uint)totalWeight);
            int accumulatedWeight = 0;

            for (int i = 0; i < largeDecorations.Count; i++)
            {
                LargeDecorationRule rule = largeDecorations[i];
                if (rule == null || !rule.IsUsable)
                    continue;

                accumulatedWeight += rule.Weight;
                if (target < accumulatedWeight)
                    return rule;
            }

            return null;
        }
    }
}
