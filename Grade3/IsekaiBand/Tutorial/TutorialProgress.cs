using _Code.LCH._02.Scripts.Bus;

namespace _Work.CHUH.Code.Tutorial
{
    internal enum TutorialStep
    {
        Introduction, Movement, Dash, FirstCombat, LevelUpCombat, CardSelection,
        CombinationUnavailable, Combination, CombinedCombat, Complete
    }

    internal class TutorialProgress
    {
        public TutorialStep Step { get; set; }
        public bool DashPerformed { get; set; }
        public bool LevelUpTriggered { get; private set; }
        public bool CardSelected { get; private set; }
        public int KillCount { get; private set; }
        public int CombinedCombatKillCount { get; set; }
        public bool CombinationCompleted { get; set; }

        public void HandleDash(DashEvent evt)
        {
            if (Step == TutorialStep.Dash) DashPerformed = true;
        }

        public void RecordKill()
        {
            KillCount++;
            if (Step == TutorialStep.CombinedCombat) CombinedCombatKillCount++;
        }

        public void HandleLevelUp(LevelUpEvent evt)
        {
            if (Step == TutorialStep.LevelUpCombat) LevelUpTriggered = true;
        }

        public void HandleCardSelect(CardSelectEvent evt)
        {
            if (Step == TutorialStep.CardSelection) CardSelected = true;
        }
    }
}
