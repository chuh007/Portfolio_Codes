using System.Collections.Generic;
using _Work.CHUH.Code.Combat.Pattern;
using _Work.CHUH.Code.Enemies.Boss;

internal static class BossPatternSelection
{
    private sealed class SelectionState
    {
        public BasePatternSO SelectedPattern;
        public BasePatternSO LastPattern;
        public long CompletedPatternCount;
        public readonly Dictionary<BasePatternSO, long> AvailableAfterPatternCounts = new();
    }

    private static readonly Dictionary<Boss, SelectionState> States = new();

    public static void Set(Boss boss, BasePatternSO pattern)
    {
        if (boss == null || pattern == null) return;
        GetState(boss).SelectedPattern = pattern;
    }

    public static BasePatternSO Consume(Boss boss)
    {
        if (boss == null || !States.TryGetValue(boss, out SelectionState state)) return null;

        BasePatternSO pattern = state.SelectedPattern;
        state.SelectedPattern = null;
        return pattern;
    }

    public static void Clear(Boss boss)
    {
        if (boss == null) return;
        States.Remove(boss);
    }

    public static void ClearSelection(Boss boss)
    {
        if (boss == null || !States.TryGetValue(boss, out SelectionState state)) return;
        state.SelectedPattern = null;
        state.LastPattern = null;
    }

    public static void RecordUsed(Boss boss, BasePatternSO pattern)
    {
        if (boss == null || pattern == null) return;

        SelectionState state = GetState(boss);
        state.LastPattern = pattern;
        state.CompletedPatternCount++;
        if (pattern.SelectionCooldownPatterns > 0)
        {
            state.AvailableAfterPatternCounts[pattern] =
                state.CompletedPatternCount + pattern.SelectionCooldownPatterns;
        }
    }

    public static bool IsLastPattern(Boss boss, BasePatternSO pattern)
    {
        if (boss == null || pattern == null) return false;
        return States.TryGetValue(boss, out SelectionState state) && state.LastPattern == pattern;
    }

    public static bool IsOnCooldown(Boss boss, BasePatternSO pattern)
    {
        if (boss == null || pattern == null || pattern.SelectionCooldownPatterns <= 0) return false;
        if (!States.TryGetValue(boss, out SelectionState state)) return false;

        return state.AvailableAfterPatternCounts.TryGetValue(pattern, out long availableAfter) &&
               state.CompletedPatternCount < availableAfter;
    }

    private static SelectionState GetState(Boss boss)
    {
        if (!States.TryGetValue(boss, out SelectionState state))
        {
            state = new SelectionState();
            States.Add(boss, state);
        }

        return state;
    }
}
